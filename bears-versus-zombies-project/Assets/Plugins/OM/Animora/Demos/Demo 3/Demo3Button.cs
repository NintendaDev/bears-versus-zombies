using OM.Animora.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;

namespace OM.Animora.Demos
{
    public class Demo3Button : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IPointerClickHandler
    {
        [SerializeField] private AnimoraPlayer hoverAnimation;
        [SerializeField] private AnimoraPlayer clickAnimation;
        [SerializeField] private AnimoraPlayer releaseAnimation;


        public void OnPointerEnter(PointerEventData eventData)
        {
            clickAnimation?.StopAnimation();
            releaseAnimation?.StopAnimation();
            
            hoverAnimation?.PlayAnimation();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            hoverAnimation?.StopAnimation();
            clickAnimation?.StopAnimation();
            
            releaseAnimation?.PlayAnimation();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            hoverAnimation?.StopAnimation();
            releaseAnimation?.StopAnimation();
            
            clickAnimation?.PlayAnimation();
        }
    }
}
