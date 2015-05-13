namespace WarehouseOfMusic
{
    using Coding4Fun.Toolkit.Controls;

    public class InputPromptOveride : InputPrompt
    {
        public override void OnCompleted(PopUpEventArgs<string, PopUpResult> result)
        {
            if (result.Result == string.Empty) return;
            base.OnCompleted(result);
        }
    }
}