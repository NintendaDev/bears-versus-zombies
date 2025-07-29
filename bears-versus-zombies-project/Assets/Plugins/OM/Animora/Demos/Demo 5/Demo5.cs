using System;
using OM.Animora.Runtime;
using UnityEngine;
#if Animora_TMP
using TMPro;
#endif

namespace OM.Animora.Demos
{
    public class Demo5 : MonoBehaviour
    {
#if Animora_TMP
        [SerializeField] private TMP_Text scoreText;
#endif
        [SerializeField] private AnimoraPlayer animationPlayer;
        [SerializeField] private AnimoraPlayer completeAnimation;
        [SerializeField] private int maxScore = 10;

        private int _score;

        private void Start()
        {
            _score = 0;
            UpdateScore();
        }

        public void AddScore()
        {
            if (_score >= maxScore) return;
            
            _score++;
            UpdateScore();

            if (_score >= maxScore)
            {
                animationPlayer?.StopAnimation();
                completeAnimation?.PlayAnimation();
            }
            else
            {
                animationPlayer?.PlayAnimation();
            }
        }

        private void UpdateScore()
        {
#if Animora_TMP
            scoreText.text = _score.ToString() + "/" + maxScore;
#endif
        }
    }
}
