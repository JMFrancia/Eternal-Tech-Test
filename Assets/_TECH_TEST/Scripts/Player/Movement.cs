using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{

    public class Movement : Attribute<Controller>//, ISpawnable
    {

        [HideInInspector]
        public bool pogoMode { get; private set; } = false;

        [Header("Pogo")]
        [SerializeField] bool testGodBounce = false; //When active, god bounce will occur on first good bounce
        [SerializeField] float basePogoVel = .5f;
        [SerializeField] float pogoVelMult = 1.3f;
        [SerializeField] float pogoBounceTime = .2f;
        [SerializeField] float pogoInputWindow = .2f;
        [SerializeField] int godBounce = 7;
        [SerializeField] float godSequenceStartDelay = 1.5f;
        [SerializeField] float dropOffHeight = 10f;  //Parachute mode scrapped
        [SerializeField] bool parachuteMode = false;
        [SerializeField] float parachuteGravityMultiplier = .9f;

        [SerializeField] Text heightText; //For testing
        [SerializeField] Text pogoBounceLevelText;
        [SerializeField] GodSceneManager godSceneManager;
        [SerializeField] CameraManager cameraManager;
        [SerializeField] Transform world;
        [SerializeField] GameObject jamieObj;
        [SerializeField] GameObject pogoJamieObj;
        [SerializeField] AudioClip targetSound;
        [SerializeField] AudioClip sparkleSound;
        [SerializeField] ParticleSystem sparkleParticle;

        bool oldIsGrounded = true;
        bool oldPogoMode = false;
        int bounceCount = 0;
        int maxBounceCount = 1;
        float pogoTime = 0f;
        float groundStart;
        float groundEnd;
        float maxHeight;
        bool waitingOnBounce = false;
        bool bounced = false;
        float originalCamRadius;

        Vector3 lastGroundedPos; 
        Coroutine pogoCoroutine;
        Appearance appearance;
        Sounds sounds;
        PogoBounceState bounceState = PogoBounceState.Neutral;
        GameObject pogoCollectible;

        enum PogoBounceState {
            Good,
            Bad,
            Neutral,
            Mega
        }

        private void OnEnable()
        {
            EventManager.StartListening(Constants.EventNames.RELEASE_TOUCH, OnTouchReleased);
        }

        private void OnDisable()
        {
            EventManager.StopListening(Constants.EventNames.RELEASE_TOUCH, OnTouchReleased);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Constants.TagNames.POGO_COLLECTIBLE)) {
                pogoMode = true;
                pogoCollectible = other.gameObject;
                pogoCollectible.SetActive(false);
                Destroy(other.gameObject);
                return;
            }

            if (pogoMode && other.CompareTag(Constants.TagNames.POGO_TARGET)) {
                PogoTargetManager.Instance.NextTarget();
                maxBounceCount++;
                sounds.PlaySound(targetSound, 1f);
                UpdatePogoBounceLevelText();
                Debug.Log("Max bounce increased!");
            }
        }

        void OnTouchReleased(float timeHeld) {
            if (!pogoMode)
                return;

            bool goodRelease = (groundEnd - pogoInputWindow) <= pogoTime && pogoTime <= (groundEnd + pogoInputWindow);

            if(goodRelease)
            {
                Debug.Log("Good bounce!");
                bounceState = PogoBounceState.Good;

                if (bounceCount == godBounce || testGodBounce) {
                    basePogoVel *= testGodBounce ? 100f : 10f;
                    bounceState = PogoBounceState.Mega;
                    StartCoroutine(PlayGodSequence(godSequenceStartDelay));
                }
                
                if (waitingOnBounce)
                {
                    StopCoroutine(pogoCoroutine);
                    PogoBounce();
                }
                int newBounceCount = Mathf.Min(bounceCount + 1, maxBounceCount);
                if (newBounceCount > bounceCount)
                {
                    bounceCount = newBounceCount;
                    camRadius *= 1.1f;
                    UpdatePogoBounceLevelText();
                    maxHeight = 0f;
                }

            }
            else {
                Debug.Log("Bad bounce");
                bounceState = PogoBounceState.Bad;
            }
        }

        IEnumerator PlayGodSequence(float delay) {
            yield return new WaitForSeconds(delay);
            godSceneManager.gameObject.SetActive(true);
            basePogoVel /= 10f;
            godSceneManager.BeginSequence((System.Action)(() => {
                localVel.y = 5f;
                //parachuteMode = true;
                gameObject.SetActive(true);
                cameraManager.Set(mainCam);
                godSceneManager.gameObject.SetActive(false);
                transform.position = lastGroundedPos + (lastGroundedPos - world.position).normalized * dropOffHeight; 
                pogoMode = false;
                ResetPogo();
            }));
            gameObject.SetActive(false);
        }

        void UpdatePogoBounceLevelText() {
            pogoBounceLevelText.text = $"{bounceCount} / {maxBounceCount}";
        }

        void OnPogoLaunched() {
            groundEnd = pogoTime;
        }

        void OnPogoGrounded() {
            groundStart = pogoTime;
            waitingOnBounce = false;
            lastGroundedPos = transform.position;
        }

        void UpdatePogo() {
            if (pogoMode)
            {
                pogoTime += Time.deltaTime;
                float height = (transform.position - lastGroundedPos).magnitude;
                heightText.text = height.ToString();
                if (height > maxHeight) {
                    maxHeight = height;
                }
            }

            if (bounced && (pogoTime - groundEnd) > pogoInputWindow) {
                bounced = false;
            }

            if (oldPogoMode != pogoMode)
            {
                ResetPogo();
            }
        }

        void ResetPogo() {
            pogoTime = 0f;
            bounceCount = 0;
            waitingOnBounce = false;
            bounced = false;
            oldPogoMode = pogoMode;

            if (pogoMode)
            {
                appearance.ChangeAvatar(pogoJamieObj, .3f);
                PogoTargetManager.Instance.SetActive(true);
                originalCamRadius = camRadius;
                pogoBounceLevelText.transform.parent.gameObject.SetActive(true);
                UpdatePogoBounceLevelText();
            }
            else
            {
                appearance.ChangeAvatar(jamieObj, .25f);
                camRadius = originalCamRadius;
                pogoBounceLevelText.transform.parent.gameObject.SetActive(false);
                PogoTargetManager.Instance.SetActive(false);
                pogoCollectible.SetActive(true);
            }
        }

        void CheckPogoBounce() {
            if (!waitingOnBounce)
            {
                waitingOnBounce = true;
                pogoCoroutine = StartCoroutine(TimedPogoBounce());
            }
        } 

        IEnumerator TimedPogoBounce() {
            yield return new WaitForSeconds(pogoBounceTime);
            PogoBounce();
        }

        void PogoBounce() {
            localVel.y = basePogoVel * Mathf.Pow(pogoVelMult, bounceCount + 1);
            localVel.z = finalJoystickVel * movSpeed;
            bounced = true;
            switch (bounceState) {
                case PogoBounceState.Neutral:
                    sounds.RegularBounce();
                    break;
                case PogoBounceState.Good:
                    sounds.GoodBounce();
                    break;
                case PogoBounceState.Bad:
                    sounds.BadBounce();
                    break;
                case PogoBounceState.Mega:
                    sounds.GodBounce();
                    StartCoroutine(SparkleAfterDelay(.5f));
                    break;
            }
            bounceState = PogoBounceState.Neutral;
        }

        IEnumerator SparkleAfterDelay(float delay) {
            yield return new WaitForSeconds(delay);
            sparkleParticle.Play();
            yield return new WaitForSeconds(.3f);
            sounds.PlaySound(sparkleSound, 3f);
        }

        


        [Header("julian new mov")]
        [SerializeField] Transform sub;
        Rigidbody rb;
        public Transform Sub
        {
            get
            {
                return sub;
            }
            set
            {
                sub = value;
            }
        }

        float groundDistance = 2f;

        RaycastHit groundHit;
        public LayerMask groundMask;


        public CenterOfMass currentCenterOfMass = null;
        CapsuleCollider capsuleCol;

        Camera mainCam;
        public MovementJoystick joystick;

        public bool canMove = true, canTurn = true;

        public enum MoveState
        {
            idle,
            walking,
            running,
            jumping,
            falling,
            pogo
        }
        [SerializeField] MoveState currentMoveState;
        public bool changingCOM;

        InputHandler input;
        GestureHandler Gestures;
        RaycastHandler Raycaster;

        Animations animations;

        public Transform planetParent;
        GameObject[] planets;
        public int planetNum;

        float tsi = 0f; // Time since idle
        [SerializeField] float timeToSit = 3f;

        [SerializeField] bool overrides = false;

        protected override void onAwake()
        {
            input = InputHandler.Instance;
            Raycaster = RaycastHandler.Instance;

            rb = GetComponent<Rigidbody>();
            appearance = GetComponent<Appearance>();
            sounds = GetComponent<Sounds>();

            pogoBounceLevelText.transform.parent.gameObject.SetActive(false);
            heightText.transform.parent.gameObject.SetActive(false);
        }

        CenterOfMass[] allCentersOfMass;

        protected override void onStart()
        {
            animations = controller.Animation;

            Gestures = GestureHandler.Instance;

            capsuleCol = GetComponent<CapsuleCollider>();
            mainCam = Camera.main;

            if (joystick == null)
                joystick = FindObjectOfType<MovementJoystick>();
            joystick.MaxThreshold = walkRunThreshold;

            if (planetParent != null)
            {
                planets = new GameObject[planetParent.childCount];
                for (int i = 0; i < planets.Length; i++)
                    planets[i] = planetParent.GetChild(i).gameObject;
            }

            allCentersOfMass = FindObjectsOfType<CenterOfMass>();

            defaultGravityStrength = gravStrength;

            //print(GameDataSaveSystem.Instance.lastAtomSpawnTimestamp);
            PogoTargetManager.Instance.SetActive(false);

        }

        int activePlanet = 1;
        protected override void onUpdate()
        {
            UpdatePogo();

            currentMoveState = GetMoveState();

            ManageSwipe();

            Gestures.swipeThreshold = swipeThreshold;
        }


        float walkRunThreshold = 0.9f;
        MoveState GetMoveState()//mainly for animations and other stuff
        {
            if (pogoMode) {
                return MoveState.pogo;
            }

            MoveState stateToSet;
            if (!isGrounded || hasJumped)
            {
                if (hasJumped || localVel.y > 0)
                    stateToSet = MoveState.jumping;
                else
                    stateToSet = MoveState.falling;
            }
            else
            {
                if (canMove && joystick.Direction.magnitude > 0.01f)
                {
                    if (joystick.Direction.magnitude < walkRunThreshold)
                        stateToSet = MoveState.walking;
                    else
                        stateToSet = MoveState.running;
                }
                else
                    stateToSet = MoveState.idle;
            }

            return stateToSet;
        }

        [HideInInspector]
        public Vector3 localVel, worldVel;
        Vector3 gravDir, gravDirRaw;
        public float gravStrength = 0.05f;
        private float defaultGravityStrength;
        public float movSpeed = 0.75f;
        public float boostTimer = 0f;
        public float groundRotSpeed = 0.1f, airRotSpeed = 0.04f;
        public float jumpStrengthVer = 1f, jumpStrengthHor = 1f;
        bool hasJumped;
        public bool isGrounded;
        public bool doJump;
        public bool slippery;
        bool doBounce;
        float bounceStrength;

        bool prevCanMove;
        void FixedUpdate()
        {

            HandleRigidbody();//just dealing with some rigidbody bs here

            if (!Active) return;
            if (overrides) return;
            if (joystick == null)
            {
                Debug.LogError("Unable to find joystick, has not been assigned to player");
                return;
            }


            if (canMove)
            {
                isGrounded = SetIsGrounded();
                if (isGrounded)
                    parachuteMode = false;

                if (pogoMode && oldIsGrounded != isGrounded) {
                    if (isGrounded) {
                        OnPogoGrounded();
                    } else
                    {
                        OnPogoLaunched();
                    }
                    oldIsGrounded = isGrounded;
                }

                if (hasJumped && !isGrounded)
                    hasJumped = false;

                if (isGrounded)
                {
                    if (pogoMode)
                    {
                        CheckPogoBounce();
                        doJump = false;
                    }
                    else
                    {
                        doJump = doingSwipe && input.GetUp();
                    }
                }
                else {
                    doJump = false;
                }


                //handle some change COM stuff
                CenterOfMass newCOM = GetCurrentCenterOfMass();
                if (newCOM != currentCenterOfMass)
                    changingCOM = true;
                if (isGrounded)
                    changingCOM = false;
                currentCenterOfMass = newCOM;


                HandleRotation();
                HandleMovement();
                HandleSubStuff();


                if (joystick.active)
                    tsi = 0f;
                else
                    tsi += Time.fixedDeltaTime;
            }


            if (prevCanMove != canMove)
            {
                joystick.ready = canMove;
            }
            prevCanMove = canMove;


            //turns off wind renderers and resets gravity strength
            if (isGrounded)
            {
                gravStrength = defaultGravityStrength;

                GameObject windTrail = transform.GetChild(4).gameObject;

                if (windTrail != null)
                {
                    for (int i = 0; i < windTrail.transform.childCount; i++)
                    {
                        GameObject wind = windTrail.transform.GetChild(i).gameObject;
                        wind.GetComponent<TrailRenderer>().emitting = false;
                    }
                }

            }

        }

        bool hasWarned;
        CenterOfMass GetCurrentCenterOfMass()
        {
            if (allCentersOfMass.Length == 0)
            {
                if (!hasWarned)
                {
                    hasWarned = true;
                    Debug.LogWarning("no centers of mass!");
                }
                return null;
            }

            if (allCentersOfMass.Length == 1) return allCentersOfMass[0];

            if (isGrounded) return currentCenterOfMass;

            //else, if we're in the air figure out which planet is pulling the hardest!
            float maxPull = 0;
            int maxPullIndex = 0;
            for (int i = 0; i < allCentersOfMass.Length; i++)
            {
                float pull = allCentersOfMass[i].GetGravPull(transform.position);
                if (pull > maxPull)
                {
                    maxPull = pull;
                    maxPullIndex = i;
                }
            }

            return allCentersOfMass[maxPullIndex];
        }

        void HandleRigidbody()
        {
            if (Active && canMove)
            {
                rb.constraints = RigidbodyConstraints.None;
            }
            else
            {
                rb.constraints = RigidbodyConstraints.FreezeAll;
            }
        }

        void HandleRotation()
        {
            //align player to planets gravity
            if (currentCenterOfMass != null)
            {
                if (isGrounded)
                {
                    gravDirRaw = (transform.position - currentCenterOfMass.transform.position).normalized;
                    gravDir = gravDirRaw;
                }
                else
                {
                    gravDirRaw = (transform.position - currentCenterOfMass.transform.position).normalized;
                    gravDir = Vector3.Lerp(gravDir, gravDirRaw, 1f);
                }

            }
            else
            {
                gravDirRaw = Vector3.up;
                gravDir = gravDirRaw;
            }


            Quaternion planetRealign = Quaternion.FromToRotation(transform.up, gravDir) * transform.rotation;

            //calculate rotation based on joystick in relation to cam
            Vector3 pFwd = planetRealign * Vector3.forward;
            Vector3 jStickDir = mainCam.transform.TransformDirection(new Vector3(joystick.Direction.x, 0, joystick.Direction.y));//joystick dir in relation to cam
            if (doJump || (doingSwipe && !isGrounded))//jumping is based on swiping so must calculate direction of swipe, not joystick
            {
                jStickDir = mainCam.transform.TransformDirection(new Vector3(swipeDir.x, 0, swipeDir.y));
            }
            jStickDir = Vector3.ProjectOnPlane(jStickDir, gravDir).normalized;
            float ang = Vector3.SignedAngle(pFwd, jStickDir, gravDir);
            Quaternion targetInputRot = Quaternion.AngleAxis(ang, gravDir);
            targetInputRot *= planetRealign;

            //adjust how fast p turns around when jumping
            float rotSpeed;
            if (doJump)
                rotSpeed = 1f;
            else if (!isGrounded)
                rotSpeed = airRotSpeed;
            else
                rotSpeed = groundRotSpeed;

            rotSpeed = Mathf.Clamp01(rotSpeed);//just in case

            //print(rotSpeed);

            Quaternion finalRot = Quaternion.Lerp(planetRealign, targetInputRot, rotSpeed);
            rb.MoveRotation(finalRot);
        }

        float finalJoystickVel;
        void HandleMovement()
        {
            //have to do this cause unity rb is stupid:
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            Vector3 basePos = rb.position;

            if (!isGrounded || hasJumped)
            {
                //localVel = transform.InverseTransformVector(worldVel);

                float comOffset = 1f;
                if (currentCenterOfMass != null)
                    comOffset = currentCenterOfMass.GetGravPull(transform.position);


                if (!changingCOM)
                {
                    localVel.y -= gravStrength * comOffset;//gravity
                    if (parachuteMode) {
                        localVel.y *= parachuteGravityMultiplier;
                    }
                    localVel.z += joystick.Direction.magnitude * movSpeed * 0.08f;//so you only change slightly when in air
                    localVel.z = Mathf.Clamp(localVel.z, -movSpeed, movSpeed);//so it doesnt speed up to much
                    worldVel = transform.TransformVector(localVel);

                }
                else
                {
                    if (parachuteMode)
                    {
                        worldVel -= gravDir.normalized * gravStrength * comOffset * 2f * parachuteGravityMultiplier;//grav
                    }
                    else
                    {
                        worldVel -= gravDir.normalized * gravStrength * comOffset * 2f;//grav
                    }
                    worldVel -= worldVel.normalized * worldVel.sqrMagnitude * 0.01f;//drag
                }


                //vel.z += joystick.Direction.magnitude * movSpeed * 0.05f;//so you only change slightly when in air
                //vel.z = Mathf.Clamp(vel.z, -joystick.Direction.magnitude * movSpeed, joystick.Direction.magnitude * movSpeed);//so it doesnt speed up to much
            }
            else
            {
                localVel = Vector3.zero;


                basePos = groundHit.point + (gravDir * capsuleCol.radius * 0.5f);//give it slight offset so it doesn't collide into ground and cause that annoying drift

                //do slippery
                if (slippery)
                    finalJoystickVel = Mathf.Lerp(joystick.Direction.magnitude, finalJoystickVel, 0.995f);
                else
                    finalJoystickVel = joystick.Direction.magnitude;

                //move based on joystick
                if (currentMoveState != MoveState.running)
                    localVel.z = finalJoystickVel * movSpeed * 0.5f;
                else
                    localVel.z = finalJoystickVel * movSpeed;

                if (pogoMode)
                {
                    hasJumped = true;
                }
                else {
                    if (doJump)
                    {
                        hasJumped = true;
                        localVel.y = jumpStrengthVer * swipeSpeed;//do jump
                        localVel.z = movSpeed * jumpStrengthHor * swipeSpeed;
                    }
                    else if (doBounce)
                    {
                        doBounce = false;
                        hasJumped = true;
                        localVel.y = bounceStrength;
                        localVel.z *= 1.2f;
                    }
                }
                worldVel = transform.TransformVector(localVel);
            }


            if (pogoMode && isGrounded) {
                localVel.z = 0f;
            }



            //do actual movement
            rb.MovePosition(basePos + worldVel);
            if (!changingCOM)
                Debug.DrawLine(basePos, basePos + worldVel, Color.green, 10f);
            else
                Debug.DrawLine(basePos, basePos + worldVel, Color.red, 10f);

        }

        void HandleSubStuff()//idk if we're using this still but sub is supposed to be the avatar mesh gameobject
        {

            if (sub == null)
                return;

            if (isGrounded)
                sub.position = Vector3.Lerp(sub.position, groundHit.point, 0.3f);
            else
                sub.position = transform.position;// Vector3.Lerp(sub.position, transform.position, 0.3f);

            sub.transform.rotation = transform.rotation;
        }

        bool SetIsGrounded()
        {
            Collider prevCollider = null;
            if (isGrounded)
                prevCollider = groundHit.collider;

            Ray groundRay = new Ray(transform.position + (transform.up * 6), -transform.up);
            groundHit = new RaycastHit();

            //Debug.DrawLine(transform.position + (transform.up * 10), transform.position + (-transform.up * (10 + groundDistance)), Color.blue, 5f);
            bool g = Physics.Raycast(groundRay, out groundHit, 6 + groundDistance, groundMask.value);

            if (g && !isGrounded)//is grounded now but wasnt before
            {
                if (onGroundEnter != null) // Enter ground
                    onGroundEnter(groundHit.collider);
            }
            else if (!g && prevCollider != null)//isnt grounded now but was before
            {
                if (onGroundExit != null)
                    onGroundExit(prevCollider);//i guess this is how you want me to set it up rick?
            }

            return g;
        }

        bool doingSwipe;
        Vector3 swipeDir;
        float swipeSpeed;
        float swipeTim = 999999f;
        void ManageSwipe()
        {
            if (Gestures.GetAnySwipe() && RaycastHandler.Instance.IsSafe && joystick.ready)
            {
                swipeTim = 0f;
                doingSwipe = true;
                swipeDir = Gestures.Swipe.normalized;
                float ss = Mathf.Clamp(Gestures.Swipe.magnitude / swipeThreshold, 1f, 10f);
                swipeSpeed = Mathf.Max(swipeSpeed, ss.Remap(1f, 20f, .75f, 1.7f) * swipeSpeedJump);
            }
            else if (swipeTim < swipeBuffTime && RaycastHandler.Instance.IsSafe)
                swipeTim += Time.deltaTime;
            else
            {
                doingSwipe = false;
                swipeDir = Vector3.zero;
                swipeSpeed = 0f;
            }

            //if (doingSwipe)
            //{
            //    print("swiping! dir: " + swipeDir + ", speed: " + swipeSpeed);
            //}
        }

        public void DoBounce(float bounceForce)
        {
            doBounce = true;
            bounceStrength = bounceForce;
        }

        //----------animation accessors-----------

        public float Velocity
        {
            get
            {
                return localVel.z;
            }
        }

        public float Step
        {
            get
            {
                //if (currentMoveState == MoveState.walking)
                //{
                //    return joystick.Direction.magnitude / walkRunThreshold;
                //}
                //else if (currentMoveState == MoveState.running)
                //{
                //    return (joystick.Direction.magnitude - walkRunThreshold) / (1f - walkRunThreshold);
                //}
                //return 0f;
                return joystick.Direction.magnitude;
            }
        }

        public bool Sitting
        {
            get
            {
                return (isGrounded && tsi >= timeToSit && !controller.Posing && !controller.Static);
            }
        }

        public MoveState CurrentMoveState
        {
            get { return currentMoveState; }
        }

        public float camSpeedX = 0.4f;
        public float camSpeedY = .005f;
        public float camRadius = 40f;

        //starts at 0.01f then lerps to .15f
        public float camFollowSpeed = 0.15f;

        public int swipeThreshold = 300;
        public float swipeBuffTime = 0.3f;
        public float swipeSpeedJump = 1f;


        //some of ricks stuff i guess i had to keep :/ but im not really using them

        [HideInInspector] public Animator characterAnimator;


        public delegate void OnGroundEnter(Collider current);
        public static event OnGroundEnter onGroundEnter;

        public delegate void OnGroundExit(Collider previous);
        public static event OnGroundExit onGroundExit;


        public void Teleport(Vector3 pos)
        {
            transform.position = pos;
        }
    }

}