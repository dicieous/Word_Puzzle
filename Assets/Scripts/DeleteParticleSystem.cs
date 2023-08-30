using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteParticleSystem : MonoBehaviour
{
	private ParticleSystem _particle;
	private void Start()
	{
		_particle = GetComponent<ParticleSystem>();
	}

	private void Update()
	{
		if (!_particle.IsAlive(true))
		{
			Destroy(_particle.gameObject);
		}
	}
}
