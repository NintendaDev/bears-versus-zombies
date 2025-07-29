using Modules.SaveSystem.SaveLoad;

namespace SampleGame.App
{
    public sealed class GameFacadeSerializer : GameSerializer<GameFacade, GameFacadeData>
    {
        protected override GameFacadeData Serialize(GameFacade service)
        {
            return new GameFacadeData(service.CurrentRegion);
        }

        protected override void Deserialize(GameFacade service, GameFacadeData data)
        {
            service.SetRegion(data.CurrentRegion);
        }
    }
}