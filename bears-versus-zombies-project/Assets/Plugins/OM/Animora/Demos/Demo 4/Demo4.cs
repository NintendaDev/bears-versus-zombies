using OM.Animora.Runtime;
using UnityEngine;

namespace OM.Animora.Demos
{
    public class Demo4 : MonoBehaviour
    {
        [SerializeField] private AnimoraPlayer openContainer1;
        [SerializeField] private AnimoraPlayer openContainer2;
        [SerializeField] private AnimoraPlayer openContainer3;

        public void OpenContainer1()
        {
            openContainer2.StopAnimation();
            openContainer3.StopAnimation();
            openContainer1.PlayAnimation();
        }
        
        public void OpenContainer2()
        {
            openContainer1.StopAnimation();
            openContainer3.StopAnimation();
            openContainer2.PlayAnimation();
        }
        
        public void OpenContainer3()
        {
            openContainer1.StopAnimation();
            openContainer2.StopAnimation();
            openContainer3.PlayAnimation();
        }
    }
}
