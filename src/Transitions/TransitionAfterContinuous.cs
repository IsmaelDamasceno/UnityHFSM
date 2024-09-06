using System;

namespace UnityHFSM
{
	/// <summary>
	/// A class used to determine whether the state machine should transition to another state.
	/// The transition will occur only when the condition remains true for a predefined period of time, determined by conditionTotalTime.
	/// </summary>
	public class TransitionAfterContinuous<TStateId> : TransitionBase<TStateId>
	{
		public float conditionTotalTime;
		public ITimer timer;

		public Func<TransitionAfterContinuous<TStateId>, bool> condition;

		public Action<TransitionAfterContinuous<TStateId>> beforeTransition;
		public Action<TransitionAfterContinuous<TStateId>> afterTransition;

		/// <summary>
		/// Initialises a new instance of the TransitionAfterContinuous class.
		/// </summary>
		/// <param name="conditionTotalTime">The amount of time that must elapse with the <c>to</c> being true for the transition to <c>to</c> happen </param>
		/// <param name="condition">A function that returns true if the state machine
		/// 	should transition to the <c>to</c> state.
		/// 	It is only called every frame, but the transition will occur only if the condition is true for the amount of continuous amount of time, determined by <c>conditionTotalTime</c> .</param>
		/// <inheritdoc cref="Transition{TStateId}(TStateId, TStateId, Func{Transition{TStateId}, bool},
		/// 	Action{Transition{TStateId}}, Action{Transition{TStateId}}, bool)" />
		public TransitionAfterContinuous(
				TStateId from,
				TStateId to,
				float conditionTotalTime,
				Func<TransitionAfterContinuous<TStateId>, bool> condition = null,
				Action<TransitionAfterContinuous<TStateId>> onTransition = null,
				Action<TransitionAfterContinuous<TStateId>> afterTransition = null,
				bool forceInstantly = false) : base(from, to, forceInstantly)
		{
			this.conditionTotalTime = conditionTotalTime;
			this.condition = condition;
			this.beforeTransition = onTransition;
			this.afterTransition = afterTransition;
			this.timer = new Timer();
		}

		public override void OnEnter()
		{
			timer.Reset();
		}

		public override bool ShouldTransition()
		{
			// if no condition is provided, the state will auto transition after the delay amount of time
			if (condition == null)
				return timer.Elapsed > conditionTotalTime;

			// if a condition is provided, it will be tested every frame, and the transition should occur when delay amount of time has elapsed
			if (condition(this))
				return timer.Elapsed > conditionTotalTime;

			// if the condition is false, the timer will reset, restarting the whole process
			timer.Reset();
			return false;
		}

		public override void BeforeTransition() => beforeTransition?.Invoke(this);
		public override void AfterTransition() => afterTransition?.Invoke(this);
	}

	/// <inheritdoc />
	public class TransitionAfterContinuous : TransitionAfterContinuous<string>
	{
		/// <inheritdoc />
		public TransitionAfterContinuous(
			string @from,
			string to,
			float conditionTotalTime,
			Func<TransitionAfterContinuous<string>, bool> condition = null,
			Action<TransitionAfterContinuous<string>> onTransition = null,
			Action<TransitionAfterContinuous<string>> afterTransition = null,
			bool forceInstantly = false) : base(
				@from,
				to,
				conditionTotalTime,
				condition,
				onTransition: onTransition,
				afterTransition: afterTransition,
				forceInstantly: forceInstantly)
		{
		}
	}
}
