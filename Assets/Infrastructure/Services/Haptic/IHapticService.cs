namespace Infrastructure.Services.Haptic
{
    public interface IHapticService
    {
        public void Play(HapticType type);
    }
    
    public enum HapticType
    {
        Button = 0,
        ThermometerInteraction = 1
    }
}