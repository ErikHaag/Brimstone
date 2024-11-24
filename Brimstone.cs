using Quintessential;

namespace Brimstone;

internal class Brimstone : QuintessentialMod
{
    
    override public void Load()
    {
        Quintessential.Logger.Log("Brimstone: Loading!");
        BrimstoneAPI.Load();
        Quintessential.Logger.Log("Brimstone: Done!");
    }

    public override void PostLoad()
    {
    }

    public override void LoadPuzzleContent()
    {
    }

    public override void Unload()
    {
    }
}