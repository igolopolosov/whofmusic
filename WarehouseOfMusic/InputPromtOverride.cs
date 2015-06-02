namespace WarehouseOfMusic
{
    using Coding4Fun.Toolkit.Controls;

    public class InputPromptOveride : InputPrompt
    {
        /// <summary>
        /// Doesn't allow to enter a blank name
        /// </summary>
        /// <param name="result"></param>
        public override void OnCompleted(PopUpEventArgs<string, PopUpResult> result)
        {
            if (result.Result == string.Empty) return;
            base.OnCompleted(result);
        }
    }
}