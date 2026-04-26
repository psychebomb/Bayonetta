using BayoMod.Characters.Survivors.Bayo.Components;
using BayoMod.Characters.Survivors.Bayo.Components.Demon;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates.Emotes
{
    public class LetsDance2 : LetsDance
    {
        private CameraController cam;
        private UIController ui;
        public override void OnEnter()
        {
            zoom = false;
            cam = this.gameObject.GetComponent<CameraController>();
            ui = this.gameObject.GetComponent<UIController>();
            if (base.isAuthority)
            {
                cam.fov = 45f;
                cam.SetCam();
                ui.SetRORUIActiveState(false);
            }
            base.OnEnter();
        }

        public override void OnExit()
        {
            if (base.isAuthority)
            {
                cam.UnsetCam();
                ui.SetRORUIActiveState(true);
            }
            base.OnExit();
        }
    }
}
