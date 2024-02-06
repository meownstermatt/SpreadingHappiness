using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractProjectileController : MonoBehaviour, IPoolable
{	
	Rigidbody2D rigidbody2d;
	AbstractBobaPattern pattern = new BobaPatternDoNothing();

	//projectiles will be destroyed if they are destruction_radius away form MainCharacter
	float destruction_radius = 75.0f;
	float destruction_check_timer = 0.0f;
	float destruction_check_interval = 1.5f;
	//Suicide is a countdown based on destruction_check_timer 
	int suicide_countdown = 0;
	int suicide_interval = 10;

	// Start is called before the first frame update
	void Start()
	{
		rigidbody2d = GetComponent<Rigidbody2D>();
	}
	
	// ************************************************************************
	
	// Update is called once per frame
	void Update()
	{
		
	}

	// ************************************************************************
	private void FixedUpdate()
	{
		this.pattern.onFixedUpdate(this.rigidbody2d);
		
		destruction_check_timer -= Time.fixedDeltaTime;
		if(destruction_check_timer <= 0.0f )
		{
			destruction_check_timer += destruction_check_interval;
			suicide_countdown--;

			if (suicide_countdown <= 0)
			{
				Proxy_Destroy();
			}

			if(MainCharacterController.instance != null)
			{
				if(Vector3.Distance(MainCharacterController.instance.transform.position, transform.position) > destruction_radius)
				{
					Proxy_Destroy();
				}
			}
			
		}
	}

	// ************************************************************************
	public void SimpleLaunch(Vector2 direction, float force)
	{
		this.SetPattern(new BobaPatternSimpleMove(direction, force));
	}

	// ************************************************************************
	public void SetPattern(AbstractBobaPattern pattern)
	{
		this.pattern = pattern;
	}
	
	// ************************************************************************
	
	// Destroy the projectile on collision
	void OnCollisionEnter2D(Collision2D other)
	{
		MainCharacterController e = other.collider.GetComponent<MainCharacterController>();
		if (e != null)
		{
			OnCollisionEffect(e);
            Proxy_Destroy();
		}
	}
	
	// Called during a collision.
	public abstract void OnCollisionEffect(MainCharacterController e);

    //Proxy Destroy lets us use the Concrete Class to recycle the Pooled Instance instead of actually Destroying it.
    public abstract void Proxy_Destroy();

    public void OnPooled()
    {
        //Show in Heirarchy
        gameObject.hideFlags &= ~HideFlags.HideInHierarchy;
		suicide_countdown = suicide_interval;
		destruction_check_timer = destruction_check_interval;
    }

    public void OnUnPooled()
    {
        //Hide in Hierarchy
        gameObject.hideFlags |= HideFlags.HideInHierarchy;
    }

    public void OnPoolCreate()
    {
        gameObject.hideFlags |= HideFlags.HideInHierarchy;
    }

    public void OnPoolDestroy()
    {
    }

    public void OnPoolReset()
    {
        gameObject.hideFlags |= HideFlags.HideInHierarchy;
    }
}

