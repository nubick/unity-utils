using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace UnityTest
{
	[Serializable]
	public class AssertionComponent : MonoBehaviour
	{
		[Flags]
		public enum CheckMethod
		{
			AfterPeriodOfTime		= 1 << 0,
			Start					= 1 << 1,
			Update					= 1 << 2,
			FixedUpdate				= 1 << 3,
			LateUpdate				= 1 << 4,
			OnDestroy				= 1 << 5,
			OnEnable				= 1 << 6,
			OnDisable				= 1 << 7,
			OnControllerColliderHit	= 1 << 8,
			OnParticleCollision		= 1 << 9,
			OnJointBreak			= 1 << 10,
			OnBecameInvisible		= 1 << 11,
			OnBecameVisible			= 1 << 12,
			OnTriggerEnter			= 1 << 13,
			OnTriggerExit			= 1 << 14,
			OnTriggerStay			= 1 << 15,
			OnCollisionEnter		= 1 << 16,
			OnCollisionExit			= 1 << 17,
			OnCollisionStay			= 1 << 18,
			
#if !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2
			OnTriggerEnter2D		= 1 << 19,
			OnTriggerExit2D			= 1 << 20,
			OnTriggerStay2D			= 1 << 21,
			OnCollisionEnter2D		= 1 << 22,
			OnCollisionExit2D		= 1 << 23,
			OnCollisionStay2D		= 1 << 24,
#endif
		}

		[SerializeField] public float checkAfterTime = 1f;
		[SerializeField] public bool repeatCheckTime = true;
		[SerializeField] public float repeatEveryTime = 1f;
		[SerializeField] public int checkAfterFrames = 1;
		[SerializeField] public bool repeatCheckFrame = true;
		[SerializeField] public int repeatEveryFrame = 1;
		[SerializeField] public bool hasFailed;

		[SerializeField] public CheckMethod checkMethods = CheckMethod.Start;
		[SerializeField] private ActionBase m_ActionBase;

		[SerializeField] public int checksPerformed = 0;

		private int checkOnFrame = 0;

		public ActionBase Action
		{
			get { return m_ActionBase; }
			set
			{
				m_ActionBase = value;
				m_ActionBase.go = gameObject;
				
				m_ActionBase.thisPropertyPath = "";
				if (m_ActionBase is ComparerBase)
					(m_ActionBase as ComparerBase).otherPropertyPath = "";

			}
		}

		public void Awake ()
		{
			if (!Debug.isDebugBuild)
				Destroy (this);
			OnComponentCopy ();
		}

#if UNITY_EDITOR
		public void OnValidate ()
		{
			OnComponentCopy ();
		}
#endif

		private void OnComponentCopy ()
		{
			if (m_ActionBase == null) return;
			var oldActionList = FindObjectsOfType (typeof (AssertionComponent)).Where (o => ((AssertionComponent) o).m_ActionBase == m_ActionBase && o != this);

			//if it's not a copy but a new component don't do anything
			if (!oldActionList.Any ()) return;
			if (oldActionList.Count () > 1)
				Debug.LogWarning ("More than one refence to comparer found. This shouldn't happen");

			var oldAction = oldActionList.First() as AssertionComponent;
			m_ActionBase = oldAction.m_ActionBase.CreateCopy (oldAction.gameObject, gameObject);
		}

		public void Start ()
		{
			CheckAssertionFor (CheckMethod.Start);

			if(IsCheckMethodSelected(CheckMethod.AfterPeriodOfTime))
			{
				StartCoroutine("CheckPeriodically");
			}
			if(IsCheckMethodSelected (CheckMethod.Update))
			{
				checkOnFrame = Time.frameCount + checkAfterFrames;
			}
		}

		public IEnumerator CheckPeriodically()
		{
			yield return new WaitForSeconds(checkAfterTime);
			CheckAssertionFor(CheckMethod.AfterPeriodOfTime);
			while (repeatCheckTime)
			{
				yield return new WaitForSeconds(repeatEveryTime);
				CheckAssertionFor(CheckMethod.AfterPeriodOfTime);
			}
		}

		public bool ShouldCheckOnFrame()
		{
			if (Time.frameCount > checkOnFrame)
			{
				if (repeatCheckFrame)
					checkOnFrame += repeatEveryFrame;
				else
					checkOnFrame = Int32.MaxValue;
				return true;
			}
			return false;
		}

		public void OnDisable ()
		{
			CheckAssertionFor (CheckMethod.OnDisable);
		}

		public void OnEnable ()
		{
			CheckAssertionFor (CheckMethod.OnEnable);
		}

		public void OnDestroy ()
		{
			CheckAssertionFor (CheckMethod.OnDestroy);
		}

		public void Update ()
		{
			if (IsCheckMethodSelected(CheckMethod.Update) && ShouldCheckOnFrame ())
			{
				CheckAssertionFor (CheckMethod.Update);
			}
		}

		public void FixedUpdate ()
		{
			CheckAssertionFor(CheckMethod.FixedUpdate);
		}

		public void LateUpdate ()
		{
			CheckAssertionFor (CheckMethod.LateUpdate);
		}

		public void OnControllerColliderHit ()
		{
			CheckAssertionFor (CheckMethod.OnControllerColliderHit);
		}

		public void OnParticleCollision ()
		{
			CheckAssertionFor (CheckMethod.OnParticleCollision);
		}

		public void OnJointBreak ()
		{
			CheckAssertionFor (CheckMethod.OnJointBreak);
		}

		public void OnBecameInvisible ()
		{
			CheckAssertionFor (CheckMethod.OnBecameInvisible);
		}

		public void OnBecameVisible ()
		{
			CheckAssertionFor (CheckMethod.OnBecameVisible);
		}

		public void OnTriggerEnter ()
		{
			CheckAssertionFor (CheckMethod.OnTriggerEnter);
		}

		public void OnTriggerExit ()
		{
			CheckAssertionFor (CheckMethod.OnTriggerExit);
		}

		public void OnTriggerStay ()
		{
			CheckAssertionFor (CheckMethod.OnTriggerStay);
		}
		public void OnCollisionEnter ()
		{
			CheckAssertionFor (CheckMethod.OnCollisionEnter);
		}

		public void OnCollisionExit ()
		{
			CheckAssertionFor (CheckMethod.OnCollisionExit);
		}

		public void OnCollisionStay ()
		{
			CheckAssertionFor (CheckMethod.OnCollisionStay);
		}

#if !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2
		public void OnTriggerEnter2D ()
		{
			CheckAssertionFor (CheckMethod.OnTriggerEnter2D);
		}

		public void OnTriggerExit2D ()
		{
			CheckAssertionFor (CheckMethod.OnTriggerExit2D);
		}

		public void OnTriggerStay2D ()
		{
			CheckAssertionFor (CheckMethod.OnTriggerStay2D);
		}

		public void OnCollisionEnter2D ()
		{
			CheckAssertionFor (CheckMethod.OnCollisionEnter2D);
		}

		public void OnCollisionExit2D ()
		{
			CheckAssertionFor (CheckMethod.OnCollisionExit2D);
		}

		public void OnCollisionStay2D ()
		{
			CheckAssertionFor (CheckMethod.OnCollisionStay2D);
		}
#endif

		private void CheckAssertionFor (CheckMethod checkMethod)
		{
			if (IsCheckMethodSelected (checkMethod))
			{
				Assertions.CheckAssertions (this);
			}
		}

		public bool IsCheckMethodSelected (CheckMethod method)
		{
			return method == (checkMethods & method);
		}
	}
}
