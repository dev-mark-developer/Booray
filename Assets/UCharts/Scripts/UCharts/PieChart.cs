using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using Firebase.Firestore;
using Firebase.Extensions;

namespace UCharts
{
	public class PieChart : ChartBase
	{
		public float thickness = 5;
		private int segments = 720;
		[SerializeField] private Color32 m_BorderColor;
		public List<PieChartDataNode> m_Data = new List<PieChartDataNode>();
		[SerializeField] List<Color32> m_Colors = new List<Color32>();

		private bool m_PlayAnimation;
		private float m_PlayAnimationTimestamp;
		private float m_fillAmount = 1f;
		FirebaseFirestore db;
		string UserId;

		//void OnEnable()
		//{
		//	Debug.Log("Enablers");
		//	//StartCoroutine(DrawPieCharts());
		//}
        protected override void Awake()
		{

			base.Awake();
			PlayAnimation();
			db = FirebaseFirestore.DefaultInstance;
		}
        protected override void Start()
        {
            // if (gameObject.tag == "GameWonChart" || gameObject.tag == "GamePlayedChart")
            // {
            //StartCoroutine(DrawPieCharts());
            //}

        }
        ////      public void ShowPieCharts()
        //{
        //	StartCoroutine(DrawPieCharts());
        //}

        public IEnumerator DrawPieChart(StatsDB stats)
        {

			yield return new WaitForSeconds(1f);
			if (gameObject.tag == "FriendsGameWonChart")
			{

				m_Data[0].Value = stats.ClassicBorrayWin;
				m_Data[1].Value = stats.SpeedBetWin;
				m_Data[2].Value = stats.FullHouseWin;
				m_Data[3].Value = stats.TournamentWin;
			}
			if (gameObject.tag == "FriendsGamePlayedChart")
			{

				m_Data[0].Value = stats.ClassicBorrayWin + stats.ClassicBorrayLoss;
				m_Data[1].Value = stats.SpeedBetWin + stats.SpeedBetLoss;
				m_Data[2].Value = stats.FullHouseWin + stats.FullHouseLoss;
				m_Data[3].Value = stats.TournamentWin + stats.TournamentLoss;

			}
			if (gameObject.tag == "GameWonChart")
			{

				m_Data[0].Value = stats.ClassicBorrayWin;
				m_Data[1].Value = stats.SpeedBetWin;
				m_Data[2].Value = stats.FullHouseWin;
				m_Data[3].Value = stats.TournamentWin;
			}
			if (gameObject.tag == "GamePlayedChart")
			{

				m_Data[0].Value = stats.ClassicBorrayWin + stats.ClassicBorrayLoss;
				m_Data[1].Value = stats.SpeedBetWin + stats.SpeedBetLoss;
				m_Data[2].Value = stats.FullHouseWin + stats.FullHouseLoss;
				m_Data[3].Value = stats.TournamentWin + stats.TournamentLoss;

			}


			yield return null;

		}

		public IEnumerator DrawPieCharts()
		{


			yield return new WaitForSeconds(1f);
			switch (ReferencesHolder.Provider)
			{
				case "Email":
					UserId = PlayerPrefs.GetString(ReferencesHolder.EmailUserId);

					Debug.Log("Providerwaa " + ReferencesHolder.Provider);
					Debug.Log("Providerwaa ka side effect " + UserId);
					break;
				case "Guest":
					UserId = PlayerPrefs.GetString(ReferencesHolder.GuestUserId);
					break;
				case "Facebook":
					UserId = PlayerPrefs.GetString(ReferencesHolder.FBUserId);
					break;
				case "Google":
					UserId = ReferencesHolder.FriendId;
					break;
			}
            if (gameObject.tag == "FriendsGameWonChart" || gameObject.tag == "FriendsGamePlayedChart")
            {
                db.Collection(ReferencesHolder.FS_users_Collec).Document(ReferencesHolder.FriendId).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_Stats_Doc).GetSnapshotAsync().ContinueWithOnMainThread(task =>
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        /*Loader.SetActive(false)*/

                    }
                    if (task.IsCompleted)
                    {

                        StatsDB stats = task.Result.ConvertTo<StatsDB>();
                        ReferencesHolder.playerStats = stats;

                        if (gameObject.tag == "FriendsGameWonChart")
                        {

                            m_Data[0].Value = stats.ClassicBorrayWin;
                            m_Data[1].Value = stats.SpeedBetWin;
                            m_Data[2].Value = stats.FullHouseWin;
                            m_Data[3].Value = stats.TournamentWin;
                        }
                        if (gameObject.tag == "FriendsGamePlayedChart")
                        {

                            m_Data[0].Value = stats.ClassicBorrayWin + stats.ClassicBorrayLoss;
                            m_Data[1].Value = stats.SpeedBetWin + stats.SpeedBetLoss;
                            m_Data[2].Value = stats.FullHouseWin + stats.FullHouseLoss;
                            m_Data[3].Value = stats.TournamentWin + stats.TournamentLoss;

                        }

                    }






                });
            }
            else if(gameObject.tag== "GameWonChart"||gameObject.tag== "GamePlayedChart")
            {
                db.Collection(ReferencesHolder.FS_users_Collec).Document(UserId).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_Stats_Doc).GetSnapshotAsync().ContinueWithOnMainThread(task =>
				{
					if (task.IsFaulted || task.IsCanceled)
					{
						/*Loader.SetActive(false)*/

					}
					if (task.IsCompleted)
					{

						StatsDB stats = task.Result.ConvertTo<StatsDB>();
						ReferencesHolder.playerStats = stats;

						if (gameObject.tag == "GameWonChart")
						{

							m_Data[0].Value = stats.ClassicBorrayWin;
							m_Data[1].Value = stats.SpeedBetWin;
							m_Data[2].Value = stats.FullHouseWin;
							m_Data[3].Value = stats.TournamentWin;
						}
						if (gameObject.tag == "GamePlayedChart")
						{

							m_Data[0].Value = stats.ClassicBorrayWin + stats.ClassicBorrayLoss;
							m_Data[1].Value = stats.SpeedBetWin + stats.SpeedBetLoss;
							m_Data[2].Value = stats.FullHouseWin + stats.FullHouseLoss;
							m_Data[3].Value = stats.TournamentWin + stats.TournamentLoss;

						}
                        //if (gameObject.tag == "FriendsGameWonChart")
                        //{

                        //    m_Data[0].Value = 8;
                        //    m_Data[1].Value = 8;
                        //    m_Data[2].Value = 8;
                        //    m_Data[3].Value = 8;
                        //}
                        //if (gameObject.tag == "FriendsGamePlayedChart")
                        //{

                        //    m_Data[0].Value = 8;
                        //    m_Data[1].Value = 8;
                        //    m_Data[2].Value = 8;
                        //    m_Data[3].Value = 8;

                        //}

                    }






				});


			}




            //this.thickness = (float)Mathf.Clamp(this.thickness, 0, rectTransform.rect.width / 2);
            //if (m_PlayAnimation)
            //{
            //    m_fillAmount += Mathf.Lerp(0f, 1f, Time.deltaTime);
            //    if (Time.time - m_PlayAnimationTimestamp > 1f)
            //    {
            //        m_PlayAnimation = false;
            //        m_fillAmount = 1f;
            //    }
            //    SetVerticesDirty();
            //}
        }

		void Update()
		{


            this.thickness = (float)Mathf.Clamp(this.thickness, 0, rectTransform.rect.width / 2);
            if (m_PlayAnimation)
            {
                m_fillAmount += Mathf.Lerp(0f, 1f, Time.deltaTime);
                if (Time.time - m_PlayAnimationTimestamp > 1f)
                {
                    m_PlayAnimation = false;
                    m_fillAmount = 1f;
                }
                SetVerticesDirty();
            }
        }

		public void PlayAnimation()
		{
			if (Application.isPlaying)
			{
				m_PlayAnimation = true;
				m_PlayAnimationTimestamp = Time.time;
				m_fillAmount = 0f;
			}
		}

		private void DrawValueLines(VertexHelper vh)
		{

		}
		protected override void OnPopulateMesh(VertexHelper vh)
		{
			float outer = -rectTransform.pivot.x * rectTransform.rect.width;
			float inner = -rectTransform.pivot.x * rectTransform.rect.width + this.thickness;

			var outer1 = -rectTransform.pivot.x * rectTransform.rect.width * 0.6f;
			var inner1 = -rectTransform.pivot.x * rectTransform.rect.width * 0.6f + this.thickness;

			vh.Clear();

			Vector2 prevX = Vector2.zero;
			Vector2 prevY = Vector2.zero;
			Vector2 uv0 = new Vector2(0, 0);
			Vector2 uv1 = new Vector2(0, 1);
			Vector2 uv2 = new Vector2(1, 1);
			Vector2 uv3 = new Vector2(1, 0);
			Vector2 pos0;
			Vector2 pos1;
			Vector2 pos2;
			Vector2 pos3;

			float f = m_fillAmount;
			float degrees = 360f / segments;
			int fa = (int)((segments + 1) * f);

			var dataIndex = 0;
			var total = 0f;
			var currentValue = m_Data[0].Value;
			m_Data.ForEach(s => total += s.Value);
			var fillColor = m_Colors[0];
			for (int i = 0; i < fa; i++)
			{
				float rad = Mathf.Deg2Rad * (i * degrees);
				float c = Mathf.Cos(rad);
				float s = Mathf.Sin(rad);

				uv0 = new Vector2(0, 1);
				uv1 = new Vector2(1, 1);
				uv2 = new Vector2(1, 0);
				uv3 = new Vector2(0, 0);


				pos0 = prevX;
				pos1 = new Vector2(outer * c, outer * s);

				pos2 = new Vector2(inner * c, inner * s);
				pos3 = prevY;


				if (i > currentValue / total * segments)
				{
					if (dataIndex < m_Data.Count - 1)
					{
						dataIndex += 1;
						currentValue += m_Data[dataIndex].Value;
						fillColor = m_Colors[dataIndex % m_Colors.Count];
					}
				}
				// draw fill
				vh.AddUIVertexQuad(SetVbo(new[] { pos0, pos1, pos2 * inner1 / inner, pos3 * inner1 / inner }, new[] { uv0, uv1, uv2, uv3 }, fillColor));

				// draw outer circle
				vh.AddUIVertexQuad(SetVbo(new[] { pos0, pos1, pos2, pos3 }, new[] { uv0, uv1, uv2, uv3 }, m_BorderColor));

				// draw inner cirlce
				vh.AddUIVertexQuad(SetVbo(new[] { pos0 * outer1 / outer, pos1 * outer1 / outer, pos2 * inner1 / inner, pos3 * inner1 / inner }, new[] { uv0, uv1, uv2, uv3 }, m_BorderColor));

				prevX = pos1;
				prevY = pos2;
			}
		}

		public void SetData(List<PieChartDataNode> data)
		{
			m_Data = data;
			SetVerticesDirty();
		}
	}

	[Serializable]
	public class PieChartDataNode
	{
		public string Text;
		public float Value;
	}
}


