namespace GParse.Parsing.Lexing.Modules.Regex.AST
{
    internal interface ITreeVisitor<T>
    {
        T Visit ( Alternation alternation );

        T Visit ( Capture capture );

        T Visit ( CaptureReference reference );

        T Visit ( Literal literal );

        T Visit ( Range range );

        T Visit ( Repetition repetition );

        T Visit ( Sequence sequence );
    }
}
