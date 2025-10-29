using BayoMod.Characters.Survivors.Bayo.Components;
using BayoMod.Characters.Survivors.Bayo.Components.Demon;
using BayoMod.Characters.Survivors.Bayo.SkillStates;
using BayoMod.Characters.Survivors.Bayo.SkillStates.BaseStates;
using BayoMod.Characters.Survivors.Bayo.SkillStates.M1;
using BayoMod.Characters.Survivors.Bayo.SkillStates.PunishStates;
using BayoMod.Characters.Survivors.Bayo.SkillStates.Weave;
using BayoMod.Modules;
using BayoMod.Modules.Characters;
using BayoMod.Modules.Components;
using BayoMod.Survivors.Bayo.Components;
using BayoMod.Survivors.Bayo.SkillStates;
using EntityStates;
using R2API;
using R2API.Networking;
using R2API.Networking.Interfaces;
using RoR2;
using RoR2.Projectile;
using RoR2.Skills;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Networking;
using static BayoMod.Modules.Skins;

namespace BayoMod.Survivors.Bayo
{
    public class BayoSurvivor : SurvivorBase<BayoSurvivor>
    {
        public override string assetBundleName => "bayobundle";
        public override string bodyName => "BayoBody";
        public override string masterName => "BayoMonsterMaster";
        public override string modelPrefabName => "mdlBayonetta";
        public override string displayPrefabName => "mdlBayonettaDisplay";

        public const string BAYO_PREFIX = BayoPlugin.DEVELOPER_PREFIX + "_BAYO_";
        public override string survivorTokenPrefix => BAYO_PREFIX;

        public override BodyInfo bodyInfo => new BodyInfo
        {
            bodyName = bodyName,
            bodyNameToken = BAYO_PREFIX + "NAME",
            subtitleNameToken = BAYO_PREFIX + "SUBTITLE",

            characterPortrait = assetBundle.LoadAsset<Texture>("texBayoIcon"),
            bodyColor = Color.red,
            sortPosition = 100,

            crosshair = Asset.LoadCrosshair("Standard"),
            podPrefab = null,

            maxHealth = 110f,
            healthRegen = 1f,
            armor = 0f,

            jumpCount = 2,
        };
        public override CustomRendererInfo[] customRendererInfos => new CustomRendererInfo[]
        {
                new CustomRendererInfo
                {
                    childName = "Body",
                },
                new CustomRendererInfo
                {
                    childName = "GFeet",
                   
                },
                new CustomRendererInfo
                {
                    childName = "GHands",
                },
                new CustomRendererInfo
                {
                    childName = "LHG",
                },
                new CustomRendererInfo
                {
                    childName = "RHG",
                },
                new CustomRendererInfo
                {
                    childName = "LHO",
                },
                new CustomRendererInfo
                {
                    childName = "RHO",
                },
                new CustomRendererInfo
                {
                    childName = "Sleeves",
                },
                new CustomRendererInfo
                {
                    childName = "Chest",
                }/*,
                new CustomRendererInfo
                {
                    childName = "armrings",
                },
                new CustomRendererInfo
                {
                    childName = "belt",
                },
                new CustomRendererInfo
                {
                    childName = "bow",
                },
                new CustomRendererInfo
                {
                    childName = "bow1",
                },
                new CustomRendererInfo
                {
                    childName = "button",
                },
                new CustomRendererInfo
                {
                    childName = "clock",
                },
                new CustomRendererInfo
                {
                    childName = "core",
                },
                new CustomRendererInfo
                {
                    childName = "hands",
                },
                new CustomRendererInfo
                {
                    childName = "heels",
                },
                new CustomRendererInfo
                {
                    childName = "helmet",
                },
                new CustomRendererInfo
                {
                    childName = "legs",
                },
                new CustomRendererInfo
                {
                    childName = "pants",

                },
                new CustomRendererInfo
                {
                    childName = "sash",
                },
                new CustomRendererInfo
                {
                    childName = "top",
                },
                new CustomRendererInfo
                {
                    childName = "top1",
                },
                */
        };

        public override UnlockableDef characterUnlockableDef => BayoUnlockables.characterUnlockableDef;

        public override ItemDisplaysBase itemDisplays => null;

        //set in base classes
        public override AssetBundle assetBundle { get; protected set; }

        public override GameObject bodyPrefab { get; protected set; }
        public override CharacterBody prefabCharacterBody { get; protected set; }
        public override GameObject characterModelObject { get; protected set; }
        public override CharacterModel prefabCharacterModel { get; protected set; }
        public override GameObject displayPrefab { get; protected set; }

        public override CharacterModel displayCharacterModel { get; protected set; }

        private GameObject wtWard;

        //private GameObject pObj;

        private float lessGravity = 0.85f;

        private float lesserGravity = 0.97f;

        private uint sound = 0;

        private float xval = 4;
        private float yval = 2;
        private float zval = 4f;
        private int wtcooldur = 10;

        // wicked weave skill overrides

        internal static SkillDef tetsuSkillDef;
        internal static SkillDef stompSkillDef;
        internal static SkillDef weaveCancelSkillDef;

        internal static SkinDef defaultSkin;
        internal static SkinDef masterySkin;
        internal static SkinDef artSkin;
        public override void Initialize()
        {
            base.Initialize();
        }

        public override void InitializeCharacter()
        {
            //need the character unlockable before you initialize the survivordef
            BayoUnlockables.Init();

            Modules.Config.ReadConfig();

            bodyInfo.maxHealth = Modules.Config.hpStat.Value;
            bodyInfo.healthRegen = Modules.Config.regenStat.Value;
            bodyInfo.armor = Modules.Config.armorStat.Value;

            xval = Modules.Config.wtx.Value;
            yval = Modules.Config.wty.Value;
            zval = Modules.Config.wtz.Value;
            wtcooldur = Modules.Config.wtcoold.Value;

            base.InitializeCharacter();

            BayoStates.Init();
            BayoTokens.Init();

            BayoBuffs.Init(assetBundle);
            InitializeSkins();
            BayoAssets.Init(assetBundle);

            InitializeEntityStateMachines();
            InitializeSkills();
            InitializeCharacterMaster();

            AdditionalBodySetup();

            AddTVE();

            AddHooks();
        }

        private void AdditionalBodySetup()
        {
            AddHitboxes();
            bodyPrefab.AddComponent<PunishTracker>();
            //bodyPrefab.AddComponent<ClimaxTracker>();
            bodyPrefab.AddComponent<ABKRotator>();
            bodyPrefab.AddComponent<BayoController>();
            bodyPrefab.AddComponent<UIController>();
            bodyPrefab.AddComponent<CameraController>();
            prefabCharacterModel.gameObject.AddComponent<BayoAnimationEvents>();
            displayPrefab.transform.Find("DistantSound").gameObject.GetComponent<RTPCController>().akSoundString = "select";
        }

        public void AddHitboxes()
        {
            //example of how to create a HitBoxGroup. see summary for more details
            Prefabs.SetupHitBoxGroup(characterModelObject, "PunchGroup", "PunchHitbox");
            Prefabs.SetupHitBoxGroup(characterModelObject, "FallGroup", "FallHitbox");
            Prefabs.SetupHitBoxGroup(characterModelObject, "CoverGroup", "Envelop");
            Prefabs.SetupHitBoxGroup(characterModelObject, "CoverGroup2", "Envelop2");
            Prefabs.SetupHitBoxGroup(characterModelObject, "HeelGroup", "HeelHitbox");
        }
        public override void InitializeEntityStateMachines() 
        {
            Prefabs.ClearEntityStateMachines(bodyPrefab);

            //the main "Body" state machine has some special properties
            Prefabs.AddMainEntityStateMachine(bodyPrefab, "Body", typeof(BayoCharacterMain), typeof(EntityStates.SpawnTeleporterState));

            Prefabs.AddEntityStateMachine(bodyPrefab, "Weapon");
            Prefabs.AddEntityStateMachine(bodyPrefab, "Weapon2");
        }

        #region skills
        public override void InitializeSkills()
        {
            //remove the genericskills from the commando body we cloned
            Skills.ClearGenericSkills(bodyPrefab);
            //AddPassiveSkill();
            AddPrimarySkills();
            AddSecondarySkills();
            AddUtiitySkills();
            AddSpecialSkills();
        }

        //skip if you don't have a passive
        //also skip if this is your first look at skills
        private void AddPassiveSkill()
        {
            //option 1. fake passive icon just to describe functionality we will implement elsewhere
            bodyPrefab.GetComponent<SkillLocator>().passiveSkill = new SkillLocator.PassiveSkill
            {
                enabled = true,
                skillNameToken = BAYO_PREFIX + "PASSIVE_NAME",
                skillDescriptionToken = BAYO_PREFIX + "PASSIVE_DESCRIPTION",
                keywordToken = "KEYWORD_STUNNING",
                icon = assetBundle.LoadAsset<Sprite>("texPassiveIcon"),
            };

            //option 2. a new SkillFamily for a passive, used if you want multiple selectable passives
            GenericSkill passiveGenericSkill = Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, "PassiveSkill");
            SkillDef passiveSkillDef1 = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "BayonettaPassive",
                skillNameToken = BAYO_PREFIX + "PASSIVE_NAME",
                skillDescriptionToken = BAYO_PREFIX + "PASSIVE_DESCRIPTION",
                skillIcon = assetBundle.LoadAsset<Sprite>("texPassiveIcon"),

                //unless you're somehow activating your passive like a skill, none of the following is needed.
                //but that's just me saying things. the tools are here at your disposal to do whatever you like with

                //activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Shoot)),
                //activationStateMachineName = "Weapon1",
                //interruptPriority = EntityStates.InterruptPriority.Skill,

                //baseRechargeInterval = 1f,
                //baseMaxStock = 1,

                //rechargeStock = 1,
                //requiredStock = 1,
                //stockToConsume = 1,

                //resetCooldownTimerOnUse = false,
                //fullRestockOnAssign = true,
                //dontAllowPastMaxStocks = false,
                //mustKeyPress = false,
                //beginSkillCooldownOnSkillEnd = false,

                //isCombatSkill = true,
                //canceledFromSprinting = false,
                //cancelSprintingOnActivation = false,
                //forceSprintDuringState = false,

            });
            Skills.AddSkillsToFamily(passiveGenericSkill.skillFamily, passiveSkillDef1);
        }
        private void AddPrimarySkills()
        {
            Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, SkillSlot.Primary);

            SkillDef primarySkillDef1 = Skills.CreateSkillDef(new SkillDefInfo
            (
                   "BayoCombo",
                   BAYO_PREFIX + "PRIMARY_COMBO_NAME",
                   BAYO_PREFIX + "PRIMARY_COMBO_DESCRIPTION",
                   assetBundle.LoadAsset<Sprite>("texM1"),
                   new EntityStates.SerializableEntityStateType(typeof(Punch1)),
                   "Body",
                   false


               ));

            primarySkillDef1.mustKeyPress = true;
            primarySkillDef1.cancelSprintingOnActivation = true;

            Skills.AddPrimarySkills(bodyPrefab, primarySkillDef1);
        }

        private void AddSecondarySkills()
        {
            Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, SkillSlot.Secondary);

            SkillDef secondarySkillDef1 = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "BayoAbk",
                skillNameToken = BAYO_PREFIX + "SECONDARY_ABK_NAME",
                skillDescriptionToken = BAYO_PREFIX + "SECONDARY_ABK_DESCRIPTION",
                skillIcon = assetBundle.LoadAsset<Sprite>("texM2"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(M2Entry)),
                activationStateMachineName = "Body",
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,

                baseRechargeInterval = 5f,
                baseMaxStock = 3,

                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = false,
                mustKeyPress = true,
                beginSkillCooldownOnSkillEnd = false,

                isCombatSkill = true,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = true,
                forceSprintDuringState = false,

                keywordTokens = new string[6] { "KEYWORD_BAYO_ABK", "KEYWORD_BAYO_SPIN", "KEYWORD_BAYO_DOWN", "KEYWORD_BAYO_HEEL", "KEYWORD_BAYO_BREAK", "KEYWORD_BAYO_RISE" }
            });

            Skills.AddSecondarySkills(bodyPrefab, secondarySkillDef1);
        }

        private void AddUtiitySkills()
        {
            Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, SkillSlot.Utility);

            SkillDef utilitySkillDef1 = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "BayoEvade",
                skillNameToken = BAYO_PREFIX + "UTILITY_DODGE_NAME",
                skillDescriptionToken = BAYO_PREFIX + "UTILITY_DODGE_DESCRIPTION",
                skillIcon = assetBundle.LoadAsset<Sprite>("texUtility"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(Dodge)),
                activationStateMachineName = "Body",
                interruptPriority = EntityStates.InterruptPriority.Pain,

                baseRechargeInterval = 1.125f,
                baseMaxStock = 1,

                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = false,
                mustKeyPress = false,
                beginSkillCooldownOnSkillEnd = false,

                isCombatSkill = false,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = true,

                keywordTokens = new string[1] { "KEYWORD_BAYO_WT" }
            });

            Skills.AddUtilitySkills(bodyPrefab, utilitySkillDef1);
        }

        private void AddSpecialSkills()
        {
            Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, SkillSlot.Special);

            SkillDef specialSkillDef1 = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "WeaveEntry",
                skillNameToken = BAYO_PREFIX + "SPECIAL_WEAVEIN_NAME",
                skillDescriptionToken = BAYO_PREFIX + "SPECIAL_WEAVEIN_DESCRIPTION",
                skillIcon = assetBundle.LoadAsset<Sprite>("texSpecial"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(WeaveEntry)),
                activationStateMachineName = "Weapon",
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,

                baseRechargeInterval = 13.5f,
                baseMaxStock = 2,

                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = false,
                mustKeyPress = true,
                beginSkillCooldownOnSkillEnd = false,

                isCombatSkill = true,
                canceledFromSprinting = true,
                cancelSprintingOnActivation = true,
                forceSprintDuringState = false,

                keywordTokens = new string[3] { "KEYWORD_BAYO_TETS", "KEYWORD_BAYO_HSTOMP", "KEYWORD_BAYO_KD" }
            });

            tetsuSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "BayoTetsu",
                skillNameToken = BAYO_PREFIX + "SPECIAL_TETSU_NAME",
                skillDescriptionToken = BAYO_PREFIX + "SPECIAL_TETSU_DESCRIPTION",
                skillIcon = assetBundle.LoadAsset<Sprite>("texTetsu"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(Tetsu)),
                activationStateMachineName = "Body",
                baseMaxStock = 1,
                baseRechargeInterval = 1f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = true,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1
            });

            stompSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "BayoStomp",
                skillNameToken = BAYO_PREFIX + "SPECIAL_STOMP_NAME",
                skillDescriptionToken = BAYO_PREFIX + "SPECIAL_STOMP_DESCRIPTION",
                skillIcon = assetBundle.LoadAsset<Sprite>("texStomp"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(Stomp)),
                activationStateMachineName = "Body",
                baseMaxStock = 1,
                baseRechargeInterval = 1f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = true,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1
            });

            weaveCancelSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "WeaveExit",
                skillNameToken = BAYO_PREFIX + "SPECIAL_WEEAVEOUT_NAME",
                skillDescriptionToken = BAYO_PREFIX + "SPECIAL_WEAVEOUT_DESCRIPTION",
                skillIcon = assetBundle.LoadAsset<Sprite>("texCancel"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(WeaveDummy)),
                activationStateMachineName = "Body",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = true,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1
            });

            Skills.AddSpecialSkills(bodyPrefab, specialSkillDef1);
        }


        #endregion skills
        
        #region skins
        public override void InitializeSkins()
        {
            ModelSkinController skinController = prefabCharacterModel.gameObject.AddComponent<ModelSkinController>();
            ModelSkinController displaySkinController = displayCharacterModel.gameObject.AddComponent<ModelSkinController>();
            ChildLocator childLocator = prefabCharacterModel.GetComponent<ChildLocator>();

            CharacterModel.RendererInfo[] defaultRendererinfos = prefabCharacterModel.baseRendererInfos;
            //CharacterModel.RendererInfo[] displayRendererinfos = displayCharacterModel.baseRendererInfos;

            List<SkinDef> skins = new List<SkinDef>();

            #region DefaultSkin
            //this creates a SkinDef with all default fields
            defaultSkin = Modules.Skins.CreateSkinDef(BAYO_PREFIX + "BAYO1_SKIN_NAME",
                assetBundle.LoadAsset<Sprite>("texMainSkin"),
                defaultRendererinfos,
                prefabCharacterModel.gameObject);

            //these are your Mesh Replacements. The order here is based on your CustomRendererInfos from earlier
            //pass in meshes as they are named in your assetbundle
            //currently not needed as with only 1 skin they will simply take the default meshes
            //uncomment this when you have another skin
            defaultSkin.skinDefParams.meshReplacements = Modules.Skins.getMeshReplacements(assetBundle, defaultRendererinfos,
                "Body",
                "GunsFeet",
                "GunsHands",
                "LHandGun",
                "RHandGun",
                "LHandOpen",
                "RHandOpen",
                "Sleeves",
                "Hairrr"/*,
                "armrings",
                "belt",
                "bow",
                "bow.001",
                "button",
                "clock",
                "core",
                "hands",
                "heels",
                "helmet",
                "legs",
                "pants",
                "sash",
                "top",
                "top.001"*/);

            defaultSkin.skinDefParams.gameObjectActivations = new SkinDefParams.GameObjectActivation[]
{
                new SkinDefParams.GameObjectActivation
                {
                    gameObject = childLocator.FindChildGameObject("Chest"),
                    shouldActivate = false
                    
                }
};

            //add new skindef to our list of skindefs. this is what we'll be passing to the SkinController
            skins.Add(defaultSkin);
            #endregion

            //uncomment this when you have a mastery skin
            #region MasterySkin
            
            //creating a new skindef as we did before
            masterySkin = Modules.Skins.CreateSkinDef(BAYO_PREFIX + "BAYO2_SKIN_NAME",
                assetBundle.LoadAsset<Sprite>("texFamedSkin"),
                defaultRendererinfos,
                prefabCharacterModel.gameObject);

            //adding the mesh replacements as above. 
            //if you don't want to replace the mesh (for example, you only want to replace the material), pass in null so the order is preserved
            masterySkin.skinDefParams.meshReplacements = Modules.Skins.getMeshReplacements(assetBundle, defaultRendererinfos,
                "Body2",
                "LiB_Feet",
                "LiB_Hands",
                "LHGun2",
                "RHGun2",
                "LHOpen2",
                "RHOpen2",
                null,
                null/*,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null*/);

            //masterySkin has a new set of RendererInfos (based on default rendererinfos)
            //you can simply access the RendererInfos' materials and set them to the new materials for your skin.
            masterySkin.skinDefParams.rendererInfos[0].defaultMaterial = assetBundle.LoadMaterial("famedbody");
            masterySkin.skinDefParams.rendererInfos[1].defaultMaterial = assetBundle.LoadMaterial("lib");
            masterySkin.skinDefParams.rendererInfos[2].defaultMaterial = assetBundle.LoadMaterial("lib");
            masterySkin.skinDefParams.rendererInfos[3].defaultMaterial = assetBundle.LoadMaterial("famedbody");
            masterySkin.skinDefParams.rendererInfos[4].defaultMaterial = assetBundle.LoadMaterial("famedbody");
            masterySkin.skinDefParams.rendererInfos[5].defaultMaterial = assetBundle.LoadMaterial("famedbody");
            masterySkin.skinDefParams.rendererInfos[6].defaultMaterial = assetBundle.LoadMaterial("famedbody");
            masterySkin.skinDefParams.rendererInfos[7].defaultMaterial = assetBundle.LoadMaterial("hairrr");

            //here's a barebones example of using gameobjectactivations that could probably be streamlined or rewritten entirely, truthfully, but it works
            masterySkin.skinDefParams.gameObjectActivations = new SkinDefParams.GameObjectActivation[]
            {
                new SkinDefParams.GameObjectActivation
                {
                    gameObject = childLocator.FindChildGameObject("Sleeves"),
                    shouldActivate = false,
                }
            };
            //simply find an object on your child locator you want to activate/deactivate and set if you want to activate/deacitvate it with this skin

            skins.Add(masterySkin);

            #endregion

            #region artsyle skin

            artSkin = Modules.Skins.CreateSkinDef(BAYO_PREFIX + "ART_SKIN_NAME",
                assetBundle.LoadAsset<Sprite>("texMainSkin"),
                defaultRendererinfos,
                prefabCharacterModel.gameObject);

            //adding the mesh replacements as above. 
            //if you don't want to replace the mesh (for example, you only want to replace the material), pass in null so the order is preserved
            artSkin.skinDefParams.meshReplacements = Modules.Skins.getMeshReplacements(assetBundle, defaultRendererinfos,
                "Body2",
                "LiB_Feet",
                null,
                "LHOpen2",
                "RHOpen2",
                "LHOpen2",
                "RHOpen2",
                null,
                null/*,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null*/);

            //masterySkin has a new set of RendererInfos (based on default rendererinfos)
            //you can simply access the RendererInfos' materials and set them to the new materials for your skin.
            artSkin.skinDefParams.rendererInfos[0].defaultMaterial = assetBundle.LoadMaterial("famedbody");
            artSkin.skinDefParams.rendererInfos[1].defaultMaterial = assetBundle.LoadMaterial("lib");
            artSkin.skinDefParams.rendererInfos[2].defaultMaterial = assetBundle.LoadMaterial("lib");
            artSkin.skinDefParams.rendererInfos[3].defaultMaterial = assetBundle.LoadMaterial("famedbody");
            artSkin.skinDefParams.rendererInfos[4].defaultMaterial = assetBundle.LoadMaterial("famedbody");
            artSkin.skinDefParams.rendererInfos[5].defaultMaterial = assetBundle.LoadMaterial("famedbody");
            artSkin.skinDefParams.rendererInfos[6].defaultMaterial = assetBundle.LoadMaterial("famedbody");
            artSkin.skinDefParams.rendererInfos[7].defaultMaterial = assetBundle.LoadMaterial("hairrr");

            //here's a barebones example of using gameobjectactivations that could probably be streamlined or rewritten entirely, truthfully, but it works
            artSkin.skinDefParams.gameObjectActivations = new SkinDefParams.GameObjectActivation[]
            {
                new SkinDefParams.GameObjectActivation
                {
                    gameObject = childLocator.FindChildGameObject("Body"),
                    shouldActivate = false,
                },
                new SkinDefParams.GameObjectActivation
                {
                    gameObject = childLocator.FindChildGameObject("GFeet"),
                    shouldActivate = false,
                },
                new SkinDefParams.GameObjectActivation
                {
                    gameObject = childLocator.FindChildGameObject("GHands"),
                    shouldActivate = false,
                },
                new SkinDefParams.GameObjectActivation
                {
                    gameObject = childLocator.FindChildGameObject("LHG"),
                    shouldActivate = false,
                },
                new SkinDefParams.GameObjectActivation
                {
                    gameObject = childLocator.FindChildGameObject("RHG"),
                    shouldActivate = false,
                },
                new SkinDefParams.GameObjectActivation
                {
                    gameObject = childLocator.FindChildGameObject("LHO"),
                    shouldActivate = false,
                },
                new SkinDefParams.GameObjectActivation
                {
                    gameObject = childLocator.FindChildGameObject("RHO"),
                    shouldActivate = false,
                },
                new SkinDefParams.GameObjectActivation
                {
                    gameObject = childLocator.FindChildGameObject("Sleeves"),
                    shouldActivate = false,
                },
                new SkinDefParams.GameObjectActivation
                {
                    gameObject = childLocator.FindChildGameObject("Chest"),
                    shouldActivate = false,
                }
            };
            //simply find an object on your child locator you want to activate/deactivate and set if you want to activate/deacitvate it with this skin

            //skins.Add(artSkin);

            #endregion

            skinController.skins = skins.ToArray();
            displaySkinController.skins = skins.ToArray();
        }
        #endregion skins
        public override void InitializeCharacterMaster()
        {
            //you must only do one of these. adding duplicate masters breaks the game.

            //if you're lazy or prototyping you can simply copy the AI of a different character to be used
            Modules.Prefabs.CloneDopplegangerMaster(bodyPrefab, masterName, "Merc");

            //how to set up AI in code
            //BayoAI.Init(bodyPrefab, masterName);

            //how to load a master set up in unity, can be an empty gameobject with just AISkillDriver components
            //assetBundle.LoadMaster(bodyPrefab, masterName);
        }

        //Adds both screen overlays
        private static void AddTVE()
        {
            if (Modules.Config.overlayOn.Value)
            {
                TempVisualEffectAPI.AddTemporaryVisualEffect(BayoAssets.wtOverlay, HasWT);
            }
            TempVisualEffectAPI.AddTemporaryVisualEffect(BayoAssets.wtOverlay2, SnapOverlay);

            TempVisualEffectAPI.AddTemporaryVisualEffect(BayoAssets.spotlight, SpotOverlay);

            // TempVisualEffectAPI.AddTemporaryVisualEffect(BayoAssets.demonPP, SnapOverlay);
        }

        static bool HasWT(CharacterBody body)
        {
            return body.HasBuff(BayoBuffs.wtBuff);
        }
        static bool SnapOverlay(CharacterBody body)
        {
            return body.HasBuff(BayoBuffs.snapBuff);
        }

        static bool SpotOverlay(CharacterBody body)
        {
            return body.HasBuff(BayoBuffs.spotBuff);
        }
        private void AddHooks()
        {
            R2API.RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            On.RoR2.HealthComponent.TakeDamage += DamageHook;
            On.RoR2.CharacterBody.OnBuffFirstStackGained += WTHook;
            On.RoR2.CharacterBody.OnBuffFinalStackLost += CdHook;
            On.RoR2.CharacterMotor.FixedUpdate += WtGravityHook;
            On.RoR2.CharacterDeathBehavior.OnDeath += DeathHook;
            On.RoR2.GlobalEventManager.ProcessHitEnemy += WeaveHit;
            On.RoR2.SkillLocator.ApplyAmmoPack += WTBandHook;
            //On.RoR2.CharacterMaster.Respawn += ReviveHook;
            On.RoR2.CharacterMaster.RespawnExtraLife += ReviveHooka;
            On.RoR2.CharacterMaster.RespawnExtraLifeHealAndRevive += ReviveHookb;
            On.RoR2.CharacterMaster.RespawnExtraLifeShrine += ReviveHookc;
            On.RoR2.CharacterMaster.RespawnExtraLifeVoid += ReviveHookd;
            On.RoR2.SetStateOnHurt.SetStunInternal += PunishHook1;
            On.RoR2.SetStateOnHurt.OverrideStunInternal += PunishHook2;
            On.RoR2.SceneExitController.Begin += FreezeBayoHook;
            On.RoR2.UI.SurvivorIconController.Update += IconUpdaterHook;
        }

        private void IconUpdaterHook(On.RoR2.UI.SurvivorIconController.orig_Update orig, RoR2.UI.SurvivorIconController self)
        {
            orig(self);

            BodyIndex bayoIndex = BodyCatalog.FindBodyIndex("BayoBody");
            LocalUser localUser = self.GetLocalUser();
            UserProfile userProfile = ((localUser != null) ? localUser.userProfile : null);
            Loadout loadout = userProfile.loadout;

            if (self && self.survivorIcon && self.survivorBodyIndex == bayoIndex)
            {
                switch (loadout.bodyLoadoutManager.GetSkinIndex(bayoIndex))
                {
                    case 1:
                        self.survivorIcon.texture = assetBundle.LoadAsset<Texture>("texBayo2Icon");
                        break;
                    default:
                        self.survivorIcon.texture = assetBundle.LoadAsset<Texture>("texBayoIcon");
                        break;
                }
            }
        }

        private void FreezeBayoHook(On.RoR2.SceneExitController.orig_Begin orig, SceneExitController self)
        {
            if (Config.tpFreeze.Value)
            {
                ReadOnlyCollection<CharacterMaster> readOnlyInstancesList = CharacterMaster.readOnlyInstancesList;
                for (int i = 0; i < readOnlyInstancesList.Count; i++)
                {
                    CharacterMaster component = readOnlyInstancesList[i].GetComponent<CharacterMaster>();
                    if (component && component.GetBodyObject() && component.GetBodyObject().name.Contains("BayoBody"))
                    {
                        new SetFreezeOnBodyRequest(component.netId, 0.912f).Send(NetworkDestination.Clients);
                    }
                }
            }

            orig(self);
        }

        private void PunishHook2(On.RoR2.SetStateOnHurt.orig_OverrideStunInternal orig, SetStateOnHurt self, float duration)
        {
            if (self.targetStateMachine.GetComponentInParent<CharacterBody>().HasBuff(BayoBuffs.punishable))
            {
                if (self.targetStateMachine)
                {
                    if (self.targetStateMachine.state is PunishStun)
                    {
                        PunishStun stunState = self.targetStateMachine.state as PunishStun;
                        if (stunState.timeRemaining < duration)
                        {
                            stunState.ExtendStun(duration - stunState.timeRemaining);
                        }
                    }
                    else if (duration > 0f)
                    {
                        PunishStun stunState = new PunishStun();
                        stunState.stunDuration = duration;
                        self.targetStateMachine.SetInterruptState(stunState, InterruptPriority.Stun);
                    }
                }
                EntityStateMachine[] array = self.idleStateMachine;
                for (int i = 0; i < array.Length; i++)
                {
                    array[i].SetNextStateToMain();
                }
            }
            else
            {
                orig(self, duration);
            }
        }

        private void PunishHook1(On.RoR2.SetStateOnHurt.orig_SetStunInternal orig, SetStateOnHurt self, float duration)
        {
            if (self.targetStateMachine.GetComponentInParent<CharacterBody>().HasBuff(BayoBuffs.punishable))
            {
                if (self.targetStateMachine)
                {
                    if (self.targetStateMachine.state is PunishStun)
                    {
                        PunishStun stunState = self.targetStateMachine.state as PunishStun;
                        if(stunState.timeRemaining < duration)
                        {
                            stunState.ExtendStun(duration - stunState.timeRemaining);
                        }
                    }
                    else
                    {
                        PunishStun stunState = new PunishStun();
                        stunState.stunDuration = duration;
                        self.targetStateMachine.SetInterruptState(stunState, InterruptPriority.Stun);
                    }
                }
                EntityStateMachine[] array = self.idleStateMachine;
                for (int i = 0; i < array.Length; i++)
                {
                    array[i].SetNextStateToMain();
                }
            }
            else
            {
                orig(self, duration);
            }
        }

        #region revie hooks so annoyinggggg

        private void ReviveHookd(On.RoR2.CharacterMaster.orig_RespawnExtraLifeVoid orig, CharacterMaster self)
        {
            orig(self);

            if (self.GetBodyObject().name.Contains("BayoBody"))
            {
                Util.PlaySound("revive", self.GetBodyObject());
            }
        }

        private void ReviveHookc(On.RoR2.CharacterMaster.orig_RespawnExtraLifeShrine orig, CharacterMaster self)
        {
            orig(self);

            if (self.GetBodyObject().name.Contains("BayoBody"))
            {
                Util.PlaySound("revive", self.GetBodyObject());
            }
        }
        private void ReviveHookb(On.RoR2.CharacterMaster.orig_RespawnExtraLifeHealAndRevive orig, CharacterMaster self)
        {
            orig(self);

            if (self.GetBodyObject().name.Contains("BayoBody"))
            {
                Util.PlaySound("revive", self.GetBodyObject());
            }
        }
        private void ReviveHooka(On.RoR2.CharacterMaster.orig_RespawnExtraLife orig, CharacterMaster self)
        {
            orig(self);

            if (self.GetBodyObject().name.Contains("BayoBody"))
            {
                Util.PlaySound("revive", self.GetBodyObject());
            }
        }
        private CharacterBody ReviveHook(On.RoR2.CharacterMaster.orig_Respawn orig, CharacterMaster self, Vector3 footPosition, Quaternion rotation, bool wasRevivedMidStage)
        {

            CharacterBody body = orig(self, footPosition, rotation, wasRevivedMidStage);

            if (wasRevivedMidStage == true)
            {
                if (self.GetBodyObject().name.Contains("BayoBody"))
                {
                    Util.PlaySound("revive", self.GetBodyObject());
                }
            }
            return body;
        }

        #endregion
        private void WTBandHook(On.RoR2.SkillLocator.orig_ApplyAmmoPack orig, SkillLocator self)
        {
            
            CharacterBody body = self.gameObject.GetComponent<CharacterBody>();

            if (body.HasBuff(BayoBuffs.wtCoolDown))
            {
                body.ClearTimedBuffs(BayoBuffs.wtCoolDown.buffIndex);
            }
            if (body.HasBuff(BayoBuffs.wtBuff))
            {
                for (int k = 1; k <= 4; k++)
                {
                    body.AddTimedBuff(BayoBuffs.wtBuff, k);
                }
            }

            orig(self);
        }
        private void DeathHook(On.RoR2.CharacterDeathBehavior.orig_OnDeath orig, CharacterDeathBehavior self)
        {
            if (self.gameObject.name.Contains("BayoBody"))
            {
                Util.PlaySound("dead", self.gameObject);
            }

            orig(self);
        }

        private void WeaveHit(On.RoR2.GlobalEventManager.orig_ProcessHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {

            //if (damageInfo != null && damageInfo.inflictor != null && damageInfo.inflictor.GetComponent<ProjectileDamage>() != null)
            if (damageInfo != null && damageInfo.inflictor != null && damageInfo.inflictor.GetComponent<WickedWeave>() != null)
            {
                Util.PlaySound("hitw", victim);
                CharacterBody body = (victim ? victim.GetComponent<CharacterBody>() : null);
                bool healthCheck = body.healthComponent.combinedHealth <= body.maxHealth * 0.33f;
                if (body.HasBuff(BayoBuffs.wtDebuff) || (healthCheck && !body.isChampion))
                {
                    float num = 1f;
                    Vector3 forceVec;

                    if (body.characterMotor && body.characterMotor.mass >= 100)
                    {
                        num = body.characterMotor.mass - 100f;
                        body.characterMotor.Motor.ForceUnground();
                    }
                    if (victim.GetComponent<HealthComponent>().GetComponent<Rigidbody>() && body.rigidbody.mass >= 100)
                    {
                        num = body.rigidbody.mass - 100f;
                    }
                    
                    forceVec = (damageInfo.force/100) * num;
                    victim.GetComponent<HealthComponent>().TakeDamageForce(forceVec, alwaysApply: true, disableAirControlUntilCollision: true);
                }
                float down = damageInfo.force.normalized.y;
                if (down <= -0.75f && !body.isChampion)
                {
                    body.AddTimedBuff(BayoBuffs.punishable, 2f);
                    victim.GetComponent<HealthComponent>().GetComponent<SetStateOnHurt>()?.SetStun(2f);
                }
            }

            orig(self, damageInfo, victim);
        }

        private void WtGravityHook(On.RoR2.CharacterMotor.orig_FixedUpdate orig, CharacterMotor self)
        {

            if (self.body.HasBuff(BayoBuffs.wtDebuff) && !self.isGrounded)
            {
                if ((self.velocity.y <= 3f) && (self.velocity.y >= -3f))
                {
                    self.velocity.y *= lessGravity;
                    self.velocity.y -= Time.fixedDeltaTime * Physics.gravity.y;
                }
                if (self.velocity.y < -1f)
                {
                    self.velocity.y -= Time.fixedDeltaTime * Physics.gravity.y * 2f;
                }
                if ((self.velocity.x <= 7f) && (self.velocity.x >= -7f))
                {
                    self.velocity.x *= lessGravity;
                }
                if ((self.velocity.z <= -7f) && (self.velocity.z >= -7f))
                {
                    self.velocity.z *= lessGravity;
                }

                self.velocity.x *= lesserGravity;
                self.velocity.z *= lesserGravity;

                if (self.velocity.y < 0.5f && self.velocity.y > 0) self.velocity.y = 0.5f;
                if (self.velocity.y > -1f && self.velocity.y < 0) self.velocity.y = -1f;
                //if (self.velocity.x < 1f && self.velocity.x > 0) self.velocity.x = 1f;
                //if (self.velocity.x > -1f && self.velocity.x < 0) self.velocity.x = -1f;
                //if (self.velocity.z < 1f && self.velocity.z > 0) self.velocity.z = 1f;
                //if (self.velocity.z > -1f && self.velocity.z < 0) self.velocity.z = -1f;

            }
            orig(self);
            
        }

        private void CdHook(On.RoR2.CharacterBody.orig_OnBuffFinalStackLost orig, CharacterBody self, BuffDef buffDef)
        {
            if (NetworkServer.active && self && self.gameObject && self.gameObject.name.Contains("BayoBody"))
            {
                bool flagg = (buffDef == BayoBuffs.wtBuff);
                int alien = self.inventory.GetItemCount(RoR2Content.Items.AlienHead);
                int light = self.inventory.GetItemCount(DLC1Content.Items.HalfAttackSpeedHalfCooldowns);
                int pure = self.inventory.GetItemCount(RoR2Content.Items.LunarBadLuck);
                float cd = wtcooldur;

                for (int k = 0; k < alien; k++)
                {
                    cd *= 0.75f;
                }
                for (int k = 0; k < light; k++)
                {
                    cd *= 0.5f;
                }
                for (int k = 0; k < pure; k++)
                {
                    cd -= 2f;
                }
                if(cd < 0) cd = 0;

                SlowDownProjectiles temp = self.GetComponentInParent<SlowDownProjectiles>();
                if (temp) wtWard = temp.gameObject;

                if (flagg && wtWard)
                {
                    Object.Destroy(wtWard);
                    Util.PlaySound("wtexit", self.gameObject);
                    AkSoundEngine.StopPlayingID(sound);
                }
                if (!self.HasBuff(RoR2Content.Buffs.NoCooldowns) && flagg && !self.HasBuff(BayoBuffs.wtCoolDown))
                {
                    for (int k = 1; k <= cd; k++)
                    {
                        self.AddTimedBuff(BayoBuffs.wtCoolDown, k);
                    }
                }
            }
            if ( buffDef && buffDef == BayoBuffs.dodgeBuff)
            {
                if(self && self.gameObject)
                {
                    ModelLocator component = self.gameObject.GetComponent<ModelLocator>();
                    if (component)
                    {
                        ChildLocator component2 = component.modelTransform.GetComponent<ChildLocator>();
                        if ((bool)component2)
                        {
                            int childIndex = component2.FindChildIndex("MainHurtbox");
                            Transform trans = component2.FindChild(childIndex);
                            Vector3 origScale = trans.localScale;
                            Vector3 newScale = new Vector3((float)origScale.x / xval, (float)origScale.y / yval, (float)origScale.z / zval);
                            trans.set_localScale_Injected(ref newScale);
                        }
                    }
                }
            }

            orig(self, buffDef);
        }

        private void WTHook(On.RoR2.CharacterBody.orig_OnBuffFirstStackGained orig, CharacterBody self, BuffDef buffDef)
        {
            if (NetworkServer.active && self.gameObject && self.gameObject.name.Contains("BayoBody"))
            {
                bool shouldActivate = (buffDef == BayoBuffs.wtBuff);
                if (shouldActivate && !wtWard)
                {
                    wtWard = Object.Instantiate(BayoAssets.wardPrefab);
                    wtWard.GetComponent<TeamFilter>().teamIndex = self.teamComponent.teamIndex;
                    wtWard.GetComponent<BuffWard>().Networkradius = 25 + self.radius;
                    wtWard.GetComponent<NetworkedBodyAttachment>().AttachToGameObjectAndSpawn(self.gameObject);
                    sound = AkSoundEngine.PostEvent(1517750988, self.gameObject);
                }

                if (buffDef == RoR2Content.Buffs.NoCooldowns)
                {
                    if (self.HasBuff(BayoBuffs.wtCoolDown))
                    {
                        self.ClearTimedBuffs(BayoBuffs.wtCoolDown.buffIndex);
                    }
                }
            }
            if (buffDef == BayoBuffs.dodgeBuff)
            {
                if(self && self.gameObject)
                {
                    ModelLocator component = self.gameObject.GetComponent<ModelLocator>();
                    if (component)
                    {
                        ChildLocator component2 = component.modelTransform.GetComponent<ChildLocator>();
                        if ((bool)component2)
                        {
                            int childIndex = component2.FindChildIndex("MainHurtbox");
                            Transform trans = component2.FindChild(childIndex);
                            Vector3 origScale = trans.localScale;
                            Vector3 newScale = new Vector3((float)origScale.x * xval, (float)origScale.y * yval, (float)origScale.z * zval);
                            trans.set_localScale_Injected(ref newScale);
                        }
                    }
                }
            }
            
            orig(self, buffDef);
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, R2API.RecalculateStatsAPI.StatHookEventArgs args)
        {

            if (sender.HasBuff(BayoBuffs.armorBuff))
            {
                args.armorAdd += 100;
            }
            if (sender.HasBuff(BayoBuffs.wtBuff))
            {
                args.armorAdd += 350;
            }
            if (sender.HasBuff(BayoBuffs.wtDebuff))
            {
                args.moveSpeedReductionMultAdd += 0.975f;
                args.baseMoveSpeedAdd += -0.95f;
                args.attackSpeedReductionMultAdd += 0.975f;
            }
        }

        private void DamageHook(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {

            bool flag = (ulong)(damageInfo.damageType & DamageType.BypassArmor) != 0;

            if (self.body.HasBuff(BayoBuffs.dodgeBuff) && damageInfo.damage > 0f)
            {
                if(!flag)
                {
                    if (NetworkServer.active)
                    {
                        self.body.RemoveBuff(BayoBuffs.dodgeBuff);
                        self.body.AddTimedBuff(BayoBuffs.evadeSuccess, 0.1f);
                    }
                    damageInfo.rejected = true;
                }
            }
            else if (self.body.name.Contains("BayoBody") && damageInfo.damage > 0f)
            {
                GameObject dam = BayoAssets.damage;
                EffectManager.SimpleMuzzleFlash(dam, self.gameObject, "DamageCenter", true);
            }

            if (self.body.HasBuff(BayoBuffs.climaxed) && damageInfo.damage > 0f)
            {
                if (!flag)
                {
                    damageInfo.rejected = true;
                }
            }

            orig(self, damageInfo);
        }

    }
}