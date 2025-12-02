using BayoMod.Characters.Survivors.Bayo.Components.Demon;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates.Emotes
{
    public class LetsDance2 : LetsDance
    {
        private CameraController cam;
        public override void OnEnter()
        {
            zoom = false;
            cam = this.gameObject.GetComponent<CameraController>();
            if (base.isAuthority)
            {
                cam.fov = 45f;
                cam.SetCam();
            }
            base.OnEnter();
        }

        public override void OnExit()
        {
            if (base.isAuthority)
            {
                cam.UnsetCam();
            }
            base.OnExit();
        }
    }
}
