namespace CastleArcane.Model
{
    /// <summary>
    /// An interface for a kinematic behavior based on discrete steps.
    /// The 
    /// Raises an event once a step is taken.
    /// </summary>
    interface IDiscreteKinematics
    {
        // THESE MEMBERS MIGHT BE REMOVED FROM THE INTERFACE
        float VelocityX { get; set; }
        float VelocityY { get; set; }
        float AccelerationX { get; set; }
        float AccelerationY { get; set; }
        int StepCountX { get; }
        int StepCountY { get; }
        // #################################################

        event SteppingEventHandler? Stepping;
        event SteppedEventHandler? Stepped;
    }

    static class DiscreteKinematicsExtensions
    {
        public static void AccelerateX(this IDiscreteKinematics kinematics, float a, bool breakMovement)
        {
            kinematics.AccelerationX = a;
            if (breakMovement && float.Sign(kinematics.AccelerationX) != float.Sign(kinematics.VelocityX))
                kinematics.VelocityX = 0;
        }

        public static void AccelerateY(this IDiscreteKinematics kinematics, float a, bool breakMovement)
        {
            kinematics.AccelerationY = a;
            if (breakMovement && float.Sign(kinematics.AccelerationY) != float.Sign(kinematics.VelocityY))
                kinematics.VelocityY = 0;
        }
    }

    record StepInfo
    (
        Direction.Types Direction,
        int Counter
    );

    record SteppedEventArgs(StepInfo Step);

    record SteppingEventArgs(StepInfo Step)
    {
        public bool WasCanceled { get; private set; }

        public void Cancel() => WasCanceled = true;
    }

    delegate void SteppingEventHandler(IDiscreteKinematics sender, SteppingEventArgs args);

    delegate void SteppedEventHandler(IDiscreteKinematics sender, SteppedEventArgs args);

}
