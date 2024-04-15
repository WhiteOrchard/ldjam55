using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class VehicleController : MonoBehaviour
{
    public bool isPlayer = false;
    public bool isEnemy = true;
    public bool canUseTurbo = true;
    public GameObject playerSkin;
    public GameObject enemySkin;

    public GameObject moveParticleL;
    public GameObject moveParticleR;

    public GameObject turboParticleL;
    public GameObject turboParticleR;

	public Transform forwardAnchor;
	public Transform backAnchorL;
	public Transform backAnchorR;

    public Vector3 checkPointPos;
    public Quaternion checkPointRot;

    public Animator animator;
    private bool isMoving = false;
    private bool isTurbo = false;

    public float anchorFowardDistance = 0.05f;
	public float anchorSideDistance = 0.05f;
	public float anchorUpDistance = 0.05f;
	public float anchorBackwardDistance = 0.05f;

	float rotationSpeed = 100f;
	float trackAdjustmentSpeed = 10f;
    public float initialCameraAdjustmentTime = 0.3f;
	float cameraAdjustmentTime = 0.03f;
    Vector3 velocityForward = Vector3.zero;
    Vector3 velocityBackL = Vector3.zero;
    Vector3 velocityBackR = Vector3.zero;

    public float targetSpringConstant = 100f;
    public float springConstant = 0f;
    float hoveringBaseHeight = 0.1f;

    public Vector2 nominalVelocity = new Vector2(5.0f, 0.0f);
	public Vector2 velocity = new Vector2(0.0f, 0.0f);

    LayerMask trackLayer;

    float turboDuration = 1.0f;

    public int lastCrossedSector = 0;
    public int nextCrossedSector = 0;
    float prevSectorProgress = 0.0f;
    public float sectorProgress = 0.0f;
    int framesInReverseCount = 0;
    bool isReversing = false;

    List<float> lapTimes = new List<float>();
    float totalTime = 0;
    float bestTime = 0;

    float moveLateral = 0.0f;
    float moveForward = 0.0f;

    float prevAngle;
    float prevAngleDiff;
    public float correctiveFactor = 0.0002f;

    int countOfMissedHits = 0;

	void Start()
	{
        trackLayer = 1 << LayerMask.NameToLayer("RaceTrack");

        if (isPlayer)
        {
            playerSkin.SetActive(true);
            enemySkin.SetActive(false);
        }
        else if (isEnemy)
        {
            playerSkin.SetActive(false);
            enemySkin.SetActive(true);
        }

        velocity.x = nominalVelocity.x;
    }

    void Update()
	{
        if (!GameManager.instance)
        {
            return;
        }

        if (lastCrossedSector  == nextCrossedSector)
        {
            lastCrossedSector = GameManager.instance.GetSectorCount() - 1;
            return;
        }

        if (springConstant < targetSpringConstant && GameManager.instance.isCountdownStarted)
        {
            springConstant += targetSpringConstant * Time.deltaTime * 0.1f;
        }
        else if (springConstant > targetSpringConstant && GameManager.instance.isCountdownStarted)
        {
            springConstant = targetSpringConstant;
        }
        
        if (isPlayer)
        {
            moveLateral = Input.GetAxis("Horizontal");
            moveForward = Input.GetAxis("Vertical");

            Vector3 targetForwardAnchorPosition = transform.position + anchorFowardDistance * transform.forward;
            Vector3 targetBackAnchorLPosition = transform.position
                - anchorSideDistance * transform.right
                - anchorBackwardDistance * transform.forward
                + anchorUpDistance * transform.up;
            Vector3 targetBackAnchorRPosition = transform.position
                + anchorSideDistance * transform.right
                - anchorBackwardDistance * transform.forward
                + anchorUpDistance * transform.up;

            float actualCameraAdjustmentTime = GameManager.instance.isRaceStarted ? cameraAdjustmentTime : initialCameraAdjustmentTime;

            forwardAnchor.position = Vector3.SmoothDamp(forwardAnchor.position, targetForwardAnchorPosition, ref velocityForward, actualCameraAdjustmentTime);
            backAnchorL.position = Vector3.SmoothDamp(backAnchorL.position, targetBackAnchorLPosition, ref velocityBackL, actualCameraAdjustmentTime);
            backAnchorR.position = Vector3.SmoothDamp(backAnchorR.position, targetBackAnchorRPosition, ref velocityBackR, actualCameraAdjustmentTime);

            Debug.Log("Last crossed sector: " + lastCrossedSector.ToString() + " Next crossed sector: " + (nextCrossedSector).ToString() + " Percentage: " + getSectorProgress());
        }
        
        if (isEnemy)
        {
            moveForward = 1.0f;

            Vector3 directionToNextSector = GameManager.instance.GetSectorStartPosition(nextCrossedSector) - GameManager.instance.GetSectorStartPosition(lastCrossedSector);
            Vector3 projectOfDirectionToNextSectorInCurrentPlane = Vector3.ProjectOnPlane(directionToNextSector, transform.up);
            float angle = Vector3.SignedAngle(transform.forward, projectOfDirectionToNextSectorInCurrentPlane, transform.up);
            float angleDiff = Mathf.Abs(angle - prevAngle);

            if (angle > 10 && moveLateral < 1.0f)
            {
                moveLateral += correctiveFactor * angleDiff;
            }
            else if (angle < -10 && moveLateral > -1.0f)
            {
                moveLateral -= correctiveFactor * angleDiff;
            }
            else
            {
                moveLateral = 0.0f;
            }
            
            prevAngleDiff = angleDiff;
            prevAngle = angle;
        }

        Debug.DrawLine(GameManager.instance.GetSectorStartPosition(lastCrossedSector), GameManager.instance.GetSectorStartPosition(nextCrossedSector), Color.magenta);

        RaycastHit hit;
        bool hasHit = Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.up), out hit, 10.0f, trackLayer);

        if (hasHit)
        {
            Vector3 targetUp = hit.normal.normalized;
            Vector3 targetForward = Vector3.Cross(transform.right, hit.normal).normalized;
            Vector3 targetRight = Vector3.Cross(hit.normal, transform.forward).normalized;

            Matrix4x4 matrix = new Matrix4x4();
            matrix.SetColumn(0, targetRight);
            matrix.SetColumn(1, targetUp);
            matrix.SetColumn(2, targetForward);
            matrix[3, 3] = 1.0f;

            transform.rotation = Quaternion.Lerp(transform.rotation, matrix.rotation, trackAdjustmentSpeed * Time.deltaTime);
            transform.Rotate(transform.up, moveLateral * rotationSpeed * Time.deltaTime, Space.World);

            //Debug.DrawRay(hit.point, targetUp, Color.yellow);
            //Debug.DrawRay(transform.position, targetForward, Color.blue);
            //Debug.DrawRay(transform.position, targetRight, Color.red);
            //Debug.DrawRay(transform.position, targetUp, Color.green);

            float distanceToGround = hit.distance;
            float springForce = springConstant * (hoveringBaseHeight - distanceToGround);
            velocity.y = springForce;
            countOfMissedHits = 0;
        }
        else
        {
            countOfMissedHits += 1;
            if (countOfMissedHits > 5)
            {
                velocity.y = nominalVelocity.y;
                transform.SetPositionAndRotation(checkPointPos, checkPointRot);
            }
        }

        float actualForwardVelocity = GameManager.instance.isRaceStarted ? velocity.x : 0.0f;
            
        transform.localPosition += transform.TransformDirection(new Vector3(0, velocity.y * Time.deltaTime, moveForward * actualForwardVelocity * Time.deltaTime));

        if (!GameManager.instance.isRaceStarted)
        {
            return;
        }

        isMoving = (moveLateral != 0.0f || moveForward != 0.0f);
        animator.SetBool("isMoving", isMoving);
        animator.SetBool("isTurbo", isTurbo);
        
        if (isMoving && !moveParticleL.activeSelf && !moveParticleR.activeSelf)
        {
            moveParticleL.SetActive(true);
            moveParticleR.SetActive(true);
        }
        else if (!isMoving && moveParticleL.activeSelf && moveParticleR.activeSelf)
        {
            moveParticleL.SetActive(false);
            moveParticleR.SetActive(false);
        }

        if (isTurbo)
        {
            turboDuration -= Time.deltaTime;
        }

        if (turboDuration < 0)
        {
            turboDuration = 1;
            isTurbo = false;
            velocity.x = nominalVelocity.x;
            turboParticleL.SetActive(false);
            turboParticleR.SetActive(false);
        }

        if (lapTimes.Count > 0)
        {
            lapTimes[lapTimes.Count-1] += Time.deltaTime;
            totalTime += Time.deltaTime;
        }

        sectorProgress = getSectorProgress();
        if (isPlayer)
        {
            float progressDiff = sectorProgress - prevSectorProgress;
            if (progressDiff < 0)
            {
                framesInReverseCount++;
                if (framesInReverseCount > 10)
                {
                    isReversing = true;
                }
            }
            else
            {
                framesInReverseCount = 0;
                isReversing = false;
            }  
            prevSectorProgress = sectorProgress;
        }
    }

	public Transform getForwardAnchor()
	{
		return forwardAnchor;
	}

	public Transform getBackAnchorL()
	{
        return backAnchorL;
    }

	public Transform getBackAnchorR() 
	{
		return backAnchorR;
	}

	public Vector3 getAnchorNormal()
	{
        Vector3 v1 = forwardAnchor.position - backAnchorL.position;
		Vector3 v2 = forwardAnchor.position - backAnchorR.position;
		return Vector3.Cross(v2, v1).normalized;
    }

	public Vector3 getAnchorRight()
	{
        return (backAnchorR.position - backAnchorL.position).normalized;
    }

	public Vector3 getAnchorForward()
	{
        return (forwardAnchor.position - (backAnchorR.position + backAnchorL.position) / 2.0f).normalized;
    }

    public Vector3 getCameraPos()
    {
        return (backAnchorR.position + backAnchorL.position) / 2.0f;
    }

    public void enableTurbo(bool ultraTurbo)
    {
        if (!canUseTurbo)
            return;

        if (!isTurbo)
        {
            velocity.x = nominalVelocity.x * (ultraTurbo ? 2.5f : 2f);
            turboParticleL.SetActive(true);
            turboParticleR.SetActive(true);
        }
        isTurbo = true;
        turboDuration = 1;
    }
    public void setLastCrossedSector(int sectorNumber)
    {
        if (sectorNumber == GameManager.instance.GetNextSectorIndex(lastCrossedSector))
        {
            lastCrossedSector = sectorNumber;
            nextCrossedSector = GameManager.instance.GetNextSectorIndex(sectorNumber);
            checkPointPos = transform.position;
            checkPointRot = transform.rotation;
        }
    }

    public void forceLastCrossedSector(int sectorNumber)
    {
        lastCrossedSector = sectorNumber;
        nextCrossedSector = GameManager.instance.GetNextSectorIndex(sectorNumber);
    }

    public void newLap()
    {
        if (lapTimes.Count > 0)
        {
            if (bestTime == 0 || lapTimes.Last() < bestTime)
                bestTime = lapTimes.Last();
        }
        lapTimes.Add(0);
    }

    public List<float> getLapTimes()
    {
        return lapTimes;
    }

    public int getStartedLapsCount()
    {
        return lapTimes.Count;
    }

    public float getCurrentLapTime()
    {
        if (lapTimes.Count == 0)
            return 0;
        return lapTimes.Last();
    }

    public float getBestLapTime()
    {
        return bestTime;
    }

    public float getTotalTime()
    {
        return totalTime;
    }

    public int getLastCrossedSector()
    {
        return lastCrossedSector;
    }
    public int getNextCrossedSector()
    {
        return nextCrossedSector;
    }

    public float getSectorProgress()
    {         
        Vector3 sectorStart = GameManager.instance.GetSectorStartPosition(lastCrossedSector);
        Vector3 sectorEnd = GameManager.instance.GetSectorStartPosition(nextCrossedSector);
        Vector3 sectorLineDirection = sectorEnd - sectorStart;

        Vector3 vehicleVectorFromLastSector = transform.position - sectorStart;
        float projectionToSectorLine = Vector3.Dot(vehicleVectorFromLastSector, sectorLineDirection.normalized);
        return projectionToSectorLine / sectorLineDirection.magnitude;
    }

    public bool isInFrontOfOtherVehicle(VehicleController otherVehicle)
    {
        if (getStartedLapsCount() > otherVehicle.getStartedLapsCount())
            return true;
        else if (getStartedLapsCount() < otherVehicle.getStartedLapsCount())
            return false;

        if (getLastCrossedSector() > otherVehicle.getLastCrossedSector())
            return true;
        else if (getLastCrossedSector() < otherVehicle.getLastCrossedSector())
            return false;

        if (getSectorProgress() > otherVehicle.getSectorProgress())
            return true;
        else
            return false;   
    }

    public bool isReversingOnTrack()
    {
        return isReversing;
    }
}
