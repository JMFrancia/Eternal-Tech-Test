using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RippleDroplet {
    public Vector2 origin = Vector2.zero;

    public float m_life = 0f;
    float m_lifetime = 1f;

    
    public float lifetime {
        get{
            return Mathf.Clamp01(m_life / m_lifetime);
        }
    }


    public RippleDroplet(Vector2 origin, float lifetime){
        this.m_life = 0f;
        this.m_lifetime = lifetime;
        this.origin = origin;
    }

    public void Update(float amount){
        m_life += amount;
    }
}

public class RippleJoystick : MonoBehaviour
{
    [SerializeField] Joystick joystick;
    [SerializeField] Material material;


    ///<summary> SET APPROPRIATE ARRAY NAMES IN MATERIAL HERE </summary>
    static string stateEnumName     = "_JoystickState";
    static string inputPosName      = "_CurrentPosition";
    static string lifetimesArrName  = "_DropLifetime";
    static string originsArrName    = "_DropPoints";


    [SerializeField] float refresh = .167f;
    [SerializeField] float dropDecay = 1f, dropLifetime = 1f;

    [SerializeField] int maxDrops = 10;


    bool m_active = false;
    public bool active { 
        get{
            return m_active;
        }
        set{
            if(m_active != value){
                m_active = value;

                if(active)
                    StartCoroutine("Ripple");
                else
                    StopCoroutine("Ripple");
            }
        }
    }

    [SerializeField]
    List<RippleDroplet> droplets = new List<RippleDroplet>();

    void Update() {
        if(joystick == null)
            return;    

        float dt = Time.deltaTime;

        // Update all droplets if there are any
        if(droplets.Count > 0){
            RippleDroplet[] activeDroplets = droplets.ToArray();
            foreach(RippleDroplet drop in activeDroplets){
                if(drop != null){
                    if(drop.lifetime >= 1f)
                        droplets.Remove(drop);
                    else
                        drop.Update(dropDecay * dt);
                }
            }
        }

        UpdateMaterial();
    }

    IEnumerator Ripple(){
        while(active){
            AddDroplet();
            yield return new WaitForSeconds(refresh);
        }
    }

    void AddDroplet(){
        RippleDroplet droplet = new RippleDroplet(joystick.position, dropLifetime);
            droplets.Add(droplet);
    }

    void UpdateMaterial(){
        if(material == null)
            return;

        Vector2 position = Vector2.zero;
        int joystickState = 0;

        float[] lifetimes = new float[maxDrops];
        Vector4[] origins = new Vector4[maxDrops];

        RippleDroplet droplet = null;
        for(int i = 0; i < maxDrops; i++){
            if(i <= (droplets.Count-1))
                droplet = droplets[i];
            else 
                droplet = null;

            lifetimes[i] = (droplet == null)? 0f:droplet.lifetime;
            origins[i] = (droplet == null)? Vector2.zero:droplet.origin;
        }

        joystickState = (joystick != null)? (joystick as MovementJoystick).state:0;
        position = (joystick != null)? joystick.Direction:Vector2.zero;


        material.SetInt(stateEnumName, joystickState);
        material.SetVector(inputPosName, new Vector4(position.x, position.y, 0f, 0f));
        material.SetFloatArray(lifetimesArrName, lifetimes);
        material.SetVectorArray(originsArrName, origins);
    }
}
