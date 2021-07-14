using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PhysicsPlayground
{
    public class SoccerGame : MonoBehaviour
    {
        public int score = 0;
        public float respawnDistance = 100;
        public Vector3 spawnPoint;

        private GameObject goalie;
        private GameObject ball;
        private Text scoreboard;
        private bool resettingBall;

        void Awake()
        {
            CacheComponents();
            StartCoroutine(CheckDistance());
        }

        void OnTriggerEnter(Collider _other)
        {
            if (_other.gameObject == ball && !resettingBall)
            {
                ScoreGoal();
                SpeedUpGoalie();
                StartCoroutine(ResetBall());
            }
        }

        private void CacheComponents()
        {
            ball = GameObject.Find("SoccerBall");
            goalie = GameObject.Find("SoccerGoalie");
            scoreboard = transform.Find("Score [Canvas]").transform.Find("ScoreLabel").GetComponent<Text>();
        }
        private void ScoreGoal()
        {
            score += 1;
            scoreboard.text = score.ToString();
        }
        private void SpeedUpGoalie()
        {
            goalie.GetComponent<Animation>()[goalie.GetComponent<Animation>().clip.name].speed += 1;
        }

        IEnumerator CheckDistance()
        {
            while (true) {
                if (Vector3.Distance(transform.position, ball.transform.position) > respawnDistance && !resettingBall) {
                    StartCoroutine(ResetBall());
                }
                yield return new WaitForSeconds(2f);
            }
        }
        IEnumerator ResetBall()
        {
            resettingBall = true;
            yield return new WaitForSeconds(3f);
            ball.GetComponent<ParticleSystem>().Play();
            while (ball.GetComponent<MeshRenderer>().material.color.a > 0.01f) {
                Color cColor = ball.GetComponent<MeshRenderer>().material.color;
                Color nColor = Color.Lerp(cColor, new Vector4(1, 1, 1, 0), Time.deltaTime * 2f);
                ball.GetComponent<MeshRenderer>().material.color = nColor;
                ball.GetComponent<MeshRenderer>().material.SetFloat("_Glossiness", ball.GetComponent<MeshRenderer>().material.color.a);
                yield return null;
            }
            ball.GetComponent<ParticleSystem>().Stop();
            ball.GetComponent<MeshRenderer>().material.color = new Vector4(1, 1, 1, 1);
            ball.GetComponent<MeshRenderer>().material.SetFloat("_Glossiness", 0.6f);
            ball.transform.position = spawnPoint;
            ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
            ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            resettingBall = false;
        }
    }
}
