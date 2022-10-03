Behavior = RegisterBehavior("Crouch")
Behavior.agentType = "giantess"
Behavior.targetType = "oneself"

function Behavior:Start()
    -- Make Shortcuts to some components
    self.hipTransform = self.agent.bones.hips
    self.ik = self.agent.ik
    self.ik.enabled = true
    self.bodyEffector = self.ik.body

    -- Initial position of the hips
    self.bodyEffector.position = self.hipTransform.position
    self.bodyEffector.positionWeight = 1

    -- Target body rotation
    local bodyRotation = self.agent.transform.rotation.eulerAngles
    bodyRotation.x = 15
    self.targetRotation = Quaternion.Euler(bodyRotation)

    -- Calculate the target position of the hips, it is half the original hips height
    -- transform.InverseTransformPoint(point) changes from world space to the local space of the transform
    local localHipPosition = self.agent.transform.InverseTransformPoint(self.bodyEffector.position)
    Log("Current Position: " .. self.bodyEffector.position)
    Log("Local Position: " .. localHipPosition)

    -- Calculate halfs the hips height in local space
    local localTarget = localHipPosition - Vector3.up * localHipPosition.y * 0.5 - Vector3.forward * localHipPosition.y * 0.15
    Log("Local Target: " .. localTarget)
    
    -- transform.TransformPoint(point) changes from local space of the transform to world space 
    self.targetPosition = self.agent.transform.TransformPoint(localTarget)
    Log("World Target: " .. self.targetPosition)
    
end

function Behavior:Update()
    -- Smoothly Transition to the crouch position
    self.bodyEffector.position = Vector3.Lerp(self.bodyEffector.position, self.targetPosition, Time.deltaTime)
    self.bodyEffector.positionWeight = 1
    -- Rotate the body
    self.agent.transform.rotation = Quaternion.Slerp(self.agent.transform.rotation, self.targetRotation, Time.deltaTime)

end

function Behavior:Exit()
    -- Return the giantess to normal
    self.bodyEffector.positionWeight = 0
    local bodyRotation = self.agent.transform.rotation.eulerAngles
    bodyRotation.x = 0
    self.agent.transform.rotation = Quaternion.Euler(bodyRotation)
end