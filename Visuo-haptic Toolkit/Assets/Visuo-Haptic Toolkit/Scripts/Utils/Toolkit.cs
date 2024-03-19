using UnityEngine;

using VHToolkit.Redirection;

namespace VHToolkit {
	/// <summary>
	/// Available body redirection techniques.
	/// </summary>
	public enum BRTechnique {
		None,
		Reset,
		// Hand Redirection techniques
		Han2018TranslationalShift,
		Han2018InterpolatedReach,
		Azmandian2016Body,
		Azmandian2016Hybrid,
		Cheng2017Sparse,
		Geslain2022Polynom,
		Poupyrev1996GoGo,
		// Pseudo-haptic techiques
		Lecuyer2000Swamp,
		Samad2019Weight
	}

    /// <summary>
    /// Available world redirection techniques.
    /// </summary>
    public enum WRTechnique {
		None,
		Reset,
		Razzaque2001OverTimeRotation,
		Razzaque2001Rotational,
		Razzaque2001Curvature,
		Razzaque2001Hybrid,
		Azmandian2016World,
		Steinicke2008Translational
    }

	public enum WRStrategy {
		NoSteering,
		SteerToCenter,
		SteerToOrbit,
		SteerToMultipleTargets,
		SteerInDirection,
		APFRedirection
	}

	public class Toolkit : MonoBehaviour {
		public static Toolkit Instance { get; private set; }

		public ParametersToolkit parameters;

		private void OnEnable() {
            if (Instance == null || Instance == this) {
                Instance = this;
                // DontDestroyOnLoad(gameObject);
            }
            else {
                Destroy(this);
            }
        }

        public float CurvatureRadiusToRotationRate() => CurvatureRadiusToRotationRate(parameters.CurvatureRadius);

        public static float CurvatureRadiusToRotationRate(float radius) => 180f / (Mathf.PI * radius);
    }
}