using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BayoMod.Modules
{
    internal static class Skins
    {
        internal static SkinDef CreateSkinDef(string skinName, Sprite skinIcon, CharacterModel.RendererInfo[] defaultRendererInfos, GameObject root, UnlockableDef unlockableDef = null)
        {
            SkinDefInfo skinDefInfo = new SkinDefInfo
            {
                BaseSkins = Array.Empty<SkinDef>(),
                GameObjectActivations = new SkinDefParams.GameObjectActivation[0],
                Icon = skinIcon,
                MeshReplacements = new SkinDefParams.MeshReplacement[0],
                MinionSkinReplacements = new SkinDefParams.MinionSkinReplacement[0],
                Name = skinName,
                NameToken = skinName,
                ProjectileGhostReplacements = new SkinDefParams.ProjectileGhostReplacement[0],
                RendererInfos = new CharacterModel.RendererInfo[defaultRendererInfos.Length],
                RootObject = root,
                UnlockableDef = unlockableDef
            };

            On.RoR2.SkinDef.Awake += DoNothing;

            SkinDef skinDef = ScriptableObject.CreateInstance<RoR2.SkinDef>();
            skinDef.baseSkins = skinDefInfo.BaseSkins;
            skinDef.icon = skinDefInfo.Icon;
            skinDef.unlockableDef = skinDefInfo.UnlockableDef;
            skinDef.rootObject = skinDefInfo.RootObject;
            defaultRendererInfos.CopyTo(skinDefInfo.RendererInfos, 0);
            skinDef.skinDefParams = ScriptableObject.CreateInstance<SkinDefParams>();
            skinDef.skinDefParams.rendererInfos = skinDefInfo.RendererInfos;
            skinDef.skinDefParams.gameObjectActivations = skinDefInfo.GameObjectActivations;
            skinDef.skinDefParams.meshReplacements = skinDefInfo.MeshReplacements;
            skinDef.skinDefParams.projectileGhostReplacements = skinDefInfo.ProjectileGhostReplacements;
            skinDef.skinDefParams.minionSkinReplacements = skinDefInfo.MinionSkinReplacements;
            skinDef.nameToken = skinDefInfo.NameToken;
            skinDef.name = skinDefInfo.Name;

            On.RoR2.SkinDef.Awake -= DoNothing;

            return skinDef;
        }

        private static void DoNothing(On.RoR2.SkinDef.orig_Awake orig, RoR2.SkinDef self)
        {
        }

        internal struct SkinDefInfo
        {
            internal SkinDef[] BaseSkins;
            internal Sprite Icon;
            internal string NameToken;
            internal UnlockableDef UnlockableDef;
            internal GameObject RootObject;
            internal CharacterModel.RendererInfo[] RendererInfos;
            internal SkinDefParams.MeshReplacement[] MeshReplacements;
            internal SkinDefParams.GameObjectActivation[] GameObjectActivations;
            internal SkinDefParams.ProjectileGhostReplacement[] ProjectileGhostReplacements;
            internal SkinDefParams.MinionSkinReplacement[] MinionSkinReplacements;
            internal string Name;
        }

        private static CharacterModel.RendererInfo[] getRendererMaterials(CharacterModel.RendererInfo[] defaultRenderers, params Material[] materials)
        {
            CharacterModel.RendererInfo[] newRendererInfos = new CharacterModel.RendererInfo[defaultRenderers.Length];
            defaultRenderers.CopyTo(newRendererInfos, 0);

            for (int i = 0; i < newRendererInfos.Length; i++)
            {
                try
                {
                    newRendererInfos[i].defaultMaterial = materials[i];
                }
                catch
                {
                    Log.Error("error adding skin rendererinfo material. make sure you're not passing in too many");
                }
            }

            return newRendererInfos;
        }
        /// <summary>
        /// pass in strings for mesh assets in your bundle. pass the same amount and order based on your rendererinfos, filling with null as needed
        /// <code>
        /// myskindef.meshReplacements = Modules.Skins.getMeshReplacements(defaultRenderers,
        ///    "meshHenrySword",
        ///    null,
        ///    "meshHenry");
        /// </code>
        /// </summary>
        /// <param name="assetBundle">your skindef's rendererinfos to access the renderers</param>
        /// <param name="defaultRendererInfos">your skindef's rendererinfos to access the renderers</param>
        /// <param name="meshes">name of the mesh assets in your project</param>
        /// <returns></returns>
        internal static SkinDefParams.MeshReplacement[] getMeshReplacements(AssetBundle assetBundle, CharacterModel.RendererInfo[] defaultRendererInfos, params string[] meshes)
        {

            List<SkinDefParams.MeshReplacement> meshReplacements = new List<SkinDefParams.MeshReplacement>();

            for (int i = 0; i < defaultRendererInfos.Length; i++)
            {
                if (string.IsNullOrEmpty(meshes[i]))
                    continue;

                meshReplacements.Add(
                new SkinDefParams.MeshReplacement
                {
                    renderer = defaultRendererInfos[i].renderer,
                    mesh = assetBundle.LoadAsset<Mesh>(meshes[i])
                });
            }

            return meshReplacements.ToArray();
        }
    }
}