using Infrastructure.Services.Haptic;
using UnityEngine;

namespace Core.Level.Input
{
    public class InputStateFactory
    {
        private readonly HapticService _hapticService;

        public InputStateFactory(HapticService hapticService)
        {
            _hapticService = hapticService;
        }

        public InteractionHelper CreateInteractionHelper(RectTransform rectTransform, LevelModel levelModel)
        {
            return new InteractionHelper(rectTransform, levelModel, _hapticService);
        }

        public FillState CreateFillState(LevelModel levelModel, InteractionHelper interactionHelper)
        {
            return new FillState(levelModel, interactionHelper);
        }

        public CrossState CreateCrossState(LevelModel levelModel, InteractionHelper interactionHelper)
        {
            return new CrossState(levelModel, interactionHelper);
        }
    }
}
