using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class ParticleHandler : MonoBehaviour {

	enum TestState { Simulated, AsGameObjects, RenderedObjects }

	[SerializeField] TestState testState;
	[SerializeField] float spawnY = 4f;
	[SerializeField] float minX = 0f;
	[SerializeField] float maxX = 0f;
	[SerializeField] int particleCount = 10;
	[SerializeField] float spawnRate = 0.1f;
	[SerializeField] bool randomVelocity = false;
	[Header("")]
	[SerializeField] Sprite renderSprite;

	IEnumerator Start()
	{
		// Consistent random seed. Not really needed- Can be commented out
		// Random.InitState(1);

		// Create all particles based on parameters
		for (int i = 0; i < particleCount; i++)
		{
			Vector2 pos = new Vector2(Random.Range(minX, maxX), spawnY);
			Vector2 v = randomVelocity ? Random.insideUnitCircle : Vector2.zero;
			Color c = Color.HSVToRGB((float)i / (float)particleCount, 1f, 1f);
			SpawnParticle(i, pos, v, c);
			yield return new WaitForSeconds(spawnRate);
		}
	}

	void SpawnParticle(int count, Vector2 pos, Vector2 vel, Color color)
	{
		// Spawn particles as game objects, or
		if (testState == TestState.AsGameObjects)
		{
			ParticleGO pgo = new GameObject("Particle", typeof(ParticleGO), typeof(SpriteRenderer)).GetComponent<ParticleGO>();

			// Set visual component
			pgo.GetComponent<SpriteRenderer>().color = color;
			pgo.GetComponent<SpriteRenderer>().sprite = renderSprite;
			pgo.transform.localScale = Vector3.one * pgo.radius * 2f;

			pgo.Init(pos, vel, color);
		}
		else // as basic classes
		{
			Particle p = new Particle(pos, vel, color);
			if (testState == TestState.RenderedObjects) // Create sprite renderer for each particle, no gameObject to handle update loops
			{
				GameObject obj = new GameObject("Particle_Sprite", typeof(SpriteRenderer));
				obj.GetComponent<SpriteRenderer>().sprite = renderSprite;
				p.AddGameObject(obj);
			}
		}
	}

	void Update()
	{
		if (testState == TestState.AsGameObjects) return;
		
		for (int i = 0; i < Particle.allParticles.Count; i++)
			Particle.allParticles[i].Up();
	}

	void FixedUpdate()
	{
		if (testState == TestState.AsGameObjects) return;

		for (int i = 0; i < Particle.allParticles.Count; i++)
			Particle.allParticles[i].FixedUp();
	}

	void OnDrawGizmos()
	{
		if (testState == TestState.AsGameObjects) return;

		for (int i = 0; i < Particle.allParticles.Count; i++)
			Particle.allParticles[i].DebugDraw();
	}
}
