Stomp = RegisterBehavior("Stomp IK Test")
Stomp.agentType = "giantess"
Stomp.targetType = "micro"

--[[ Port of the agent.Stomp() option to lua, to allow more tweaks and improvements ]]

--[[ How Ik works: 
self.agent.ik.enabled: enabled/disable by setting as true or false

there are five effectors:

ik.leftFoot
ik.rightFoot
ik.leftHand
ik.rightHand
ik.body

and each one has four values (some examples)

leftFoot.position = Vector3:new(10,0,3) ( you may use the target position plus some offset)
leftFoot.positionWeight = 1
leftFoot.rotation = Quaternion:eulerAngles(0,90,0) 
leftFoot.rotationWeight = 0.5

weight means how much the inverse kinematics will be applied to the target, from 0 to 1 ( %0 to %100)
you can smooth transitions by using Mathf.Lerp (float numbers), Vector3.Lerp and Quaternion.Lerp or Quaternion.Slerp 

syntax is:
 c = Vector3.Lerp(a, b, f)
 means: from point a, to point b, and f is the percentage between those two points from 0 to 1. 
 https://docs.unity3d.com/ScriptReference/Vector3.Lerp.html

 one common use is to multiply f * Time.deltaTime

]]

function Stomp:Start()
    self.state = 0
    self.footCenter = 40
    self.maxDistance = 350
    self.maxOffset = 400
    self.minTimeToPrepare = 1.5
    self.crushSpeed = 3
    self.returnSpeed = 2
end

function Stomp:Update()
    -- things to define outside
    self.crushTarget = true
    -- Log(self.state)


    self.nextState = self.state
    local weightSpeed = 1
    local weight = 1
    -- create some variable shortcuts to avoid writing the full path like self.agent.ik.leftFoot.positionWeight 
    local targetTransform = self.target.transform
    local leftFootTransform = self.agent.bones.leftFoot
    local rightFootTransform = self.agent.bones.rightFoot
    local leftFootEffector = self.agent.ik.leftFoot
    local rightFootEffector = self.agent.ik.rightFoot
    local currentGiantessSpeed = self.agent.animation.GetSpeed();
 
    -- Idle State --
    if self.state == 0 then
        weight = 0
        leftFootEffector.position = leftFootTransform.position
        rightFootEffector.position = rightFootTransform.position
        if self.crushTarget and not self.agent.animation.IsInTransition() then
            if self:CanFootReach(targetTransform) and self:IsClose(targetTransform) then
                self.crushTarget = false
                -- Choose the closest foot to the target
                local distanceToLeft = (leftFootTransform.position - targetTransform.position).magnitude;
                local distanceToRight = (rightFootTransform.position - targetTransform.position).magnitude;
                if distanceToLeft < distanceToRight then         
                    self.activeFoot = leftFootEffector
                else 
                    self.activeFoot = rightFootEffector
                end
                self.agent.ik.enabled = true -- Enable the IK system (disable when you don't use it to save performance)
                self.startTime = Time.time
                self.nextState = 1
            end
        end
        
    end

    -- Raise Foot State --
    if self.state == 1 then
        weight = 1
        self.offsetPercentage = 1
        self.footTargetPosition = targetTransform.position + (Vector3.up * self.maxOffset - self.agent.transform.forward * self.footCenter) * self.agent.scale
        if targetTransform.parent != nil then
            self.nextState = 4
        end
        if Time.time - self.startTime > self.minTimeToPrepare / currentGiantessSpeed then
            self.nextState = 2
        end
    end

    -- Stomp State --
    if self.state == 2 then
        weight = 1
        self.offsetPercentage = Mathf.Lerp(self.offsetPercentage, 0, Time.deltaTime * self.crushSpeed * currentGiantessSpeed)
        local offset = self.maxOffset * self.offsetPercentage
        self.footTargetPosition = targetTransform.position + (Vector3.up * offset - self.agent.transform.forward * self.footCenter) * self.agent.scale
        if self.offsetPercentage < 0.2 then
            self.startTime = Time.time
            self.restPosition = self.activeFoot.position
            self.nextState = 3
        end
    end

    -- Stay in a position for a few moments --
    if self.state == 3 then
        weight = 1
        self.footTargetPosition = self.restPosition
        if Time.time - self.startTime > 1 / currentGiantessSpeed then
            self.nextState = 4
        end
    end

    -- Return foot to idle position --
    if self.state == 4 then
        weight = 0
        weightSpeed = self.returnSpeed
        if self.activeFoot.positionWeight < 0.5 then
            self.nextState = 0
        end
    end

    -- Apply the foot target position and target position weight --
    if self.state != 0 then
        -- Mathf Lerp is used to smoothly transition from one value to another
        self.activeFoot.positionWeight = Mathf.Lerp(self.activeFoot.positionWeight, weight, Time.deltaTime * weightSpeed * currentGiantessSpeed)
        self.activeFoot.position = self.footTargetPosition
    end

    self.state = self.nextState
end

function Stomp:IsClose(targetTransform)
    local distanceVector = self.agent.transform.position - targetTransform.position
    distanceVector.y = 0
    return distanceVector.magnitude < self.maxDistance * self.agent.scale
end

function Stomp:CanFootReach(targetTransform) 
    return targetTransform.parent == nil and self:FeetTargetIsInRange(targetTransform)
end

function Stomp:FeetTargetIsInRange(targetTransform)
    -- this calculates the target position from the giantess transform
    local distance = math.abs(self.agent.transform.InverseTransformPoint(targetTransform.position).y)
    -- Log(distance)
    return distance < self.maxOffset -- if target is less than 30 cm from the giantess point of view.
end

function Stomp:Exit()
    self.agent.ik.enabled = false
    -- disable ik at the end to save performance
end

