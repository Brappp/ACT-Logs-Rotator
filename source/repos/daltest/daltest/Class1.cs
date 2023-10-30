using Dalamud.Game.Command;
using Dalamud.Plugin;

public class MyPlugin : IDalamudPlugin
{
    private DalamudPluginInterface pluginInterface;

    public string Name => "My First Dalamud Plugin";

    public void Initialize(DalamudPluginInterface pluginInterface)
    {
        this.pluginInterface = pluginInterface;

        this.pluginInterface.CommandManager.AddHandler("/hello", new CommandInfo(OnHelloCommand)
        {
            HelpMessage = "Say hello to the world."
        });
    }

    private void OnHelloCommand(string command, string arguments)
    {
        this.pluginInterface.Framework.Gui.Chat.Print("Hello, world!");
    }

    public void Dispose()
    {
        this.pluginInterface.CommandManager.RemoveHandler("/hello");
    }
}
