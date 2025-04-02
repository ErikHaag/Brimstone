using Quintessential;

namespace Brimstone;

internal class Brimstone : QuintessentialMod
{
    
    override public void Load()
    {
        Quintessential.Logger.Log("Brimstone: Loaded!");
    }

    public override void PostLoad()
    {

    }

    public override void LoadPuzzleContent()
    {
        Quintessential.Logger.Log(BrimstoneAPI.GetContentPath("Brimstone"));
    }

    public override void Unload()
    {
    }
}