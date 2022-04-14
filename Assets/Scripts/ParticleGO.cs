﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleGO : MonoBehaviour {

	// Functions exactly the same as Particle, but all code execution is handled through Unity's update loop per object,
	// rather than a single update loop on one handler.

	public static List<ParticleGO> allParticles = new List<ParticleGO>();

	public static float gravity = -0.1f;

	public static float maxVelocity = 5f;
	public static float minVelocity = 0.01f;
	float sqrMinVelocity;

	public static float bottomY = -4f;
	public static float limitX = 3f;

	public float radius = 0.15f;
	public float mass = 1f;
	float xMultiplier = 1f;

	public Vector2 velocity { get; private set; }

	Vector2 prevPosition;
	Vector2 position;
	Vector2 nextPosition;

	// For debug
	Color c;

	bool ready = false;

	public void Init(Vector2 startPos, Vector2 startVelocity, Color color)
	{
		c = color;
		
		prevPosition = startPos;
		position = startPos;

		velocity = startVelocity * Time.fixedDeltaTime;
		ready = true;
	}

	void Start()
	{
		allParticles.Add(this);

		// Assign to sqrMinVelocity once.
		sqrMinVelocity = minVelocity * Time.fixedDeltaTime;
		sqrMinVelocity *= sqrMinVelocity;

		transform.position = position;
	}

	void OnDestroy()
	{
		if (allParticles.Contains(this))
			allParticles.Remove(this);
	}

	public void Update()
	{
		if (!ready) return;

		// This lerp preserves a smooth animation regardless of framerate inconsistencies
		transform.position = Vector3.Lerp(prevPosition, position, (Time.time - Time.fixedTime) / Time.fixedDeltaTime);
	}

	void FixedUpdate()
	{
		if (!ready) return;

		ApplyGravity();
		CheckCollisions();
		AdjustAndApply();
	}

	void ApplyGravity()
	{
		velocity += new Vector2(0f, gravity * Time.fixedDeltaTime);

		// Only apply to nextPosition if it is above the minVelocity. Reduces jittering.
		if (velocity.sqrMagnitude > sqrMinVelocity)
			nextPosition = position + velocity;
		else
			nextPosition = position;
	}

	void CheckCollisions()
	{
		Vector2 mainOffset = Vector2.zero;

		// Check against all other particles. Could be optimized with quadtrees.
		// Another project for another day.
		for (int i = 0; i < allParticles.Count; i++)
		{
			if (allParticles[i] == this) continue;

			float distance = (radius + allParticles[i].radius); // min Distance for a collision to register
			Vector2 dirVec = (nextPosition - allParticles[i].position); // Current distance between two particles
			float resultForce = distance - dirVec.magnitude;

			if (resultForce > 0f) // else, the two particles don't collide
			{
				Vector2 offset = dirVec.normalized * resultForce;
				//offset += allParticles[i].velocity
				mainOffset += offset;
			}
		}

		// Adjust impact to account for boundaries
		Vector2 boundaryAdjust = Vector2.zero;
		boundaryAdjust.y = Mathf.Max(0f, bottomY - nextPosition.y);
		boundaryAdjust.x = Mathf.Max(0f, Mathf.Abs(nextPosition.x) - limitX) * -Mathf.Sign(nextPosition.x);
		mainOffset += boundaryAdjust;

		mainOffset.x *= xMultiplier;

		// Reduces jittering
		if (mainOffset.sqrMagnitude > sqrMinVelocity)
			nextPosition += mainOffset;

		// Set new velocity direction, keep velocity magnitude
		velocity = (mainOffset.normalized + velocity.normalized) * velocity.magnitude;
	}

	// Set positions, clamp velocity magnitude
	void AdjustAndApply()
	{
		prevPosition = position;
		position = nextPosition;

		velocity = velocity.normalized * Mathf.Min(maxVelocity * Time.fixedDeltaTime, velocity.magnitude);
	}

	// For gizmos
	void OnDrawGizmos()
	{
		Gizmos.color = c;
		Gizmos.DrawSphere(transform.position, radius);	
	}
}
