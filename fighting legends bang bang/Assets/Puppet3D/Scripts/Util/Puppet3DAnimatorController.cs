using UnityEngine;
using System.Collections;
namespace Puppet3D
{
	public class Puppet3DAnimatorController : MonoBehaviour
	{
		private Animator _animator;
		//private GlobalControl _globalControl;

		public float speed = 1.0f;

		private string axisName = "Horizontal";
		// Use this for initialization
		void Start()
		{
			_animator = gameObject.GetComponent<Animator>();
			//_globalControl = gameObject.GetComponent<GlobalControl>();
		}

		// Update is called once per frame
		void Update()
		{


			if (Input.GetAxis(axisName) < 0)
			{
				_animator.SetFloat("Speed", 1);
				transform.position += transform.right * speed * Time.deltaTime;

			}
			else if (Input.GetAxis(axisName) > 0)
			{
				_animator.SetFloat("Speed", 1);
				transform.position += transform.right * speed * Time.deltaTime;

			}
			else
				_animator.SetFloat("Speed", 0);





		}

	}
}
