namespace Netric.Configuration.Console
{
    public class HelpCommand : Command
    {
        private const string Text =
            @"HELP:

  -asm Command
  
  I. General rules
    :: Command requires single argument that contains phrases.
    :: Phrases are separated by semicolon.
    :: Phrase can be assembly name or text which assembly name starts with.
    :: Assembly name is given without .dll extension.
    :: Asterisk character in the end of a phrase indicates assembly prefix to search.
    :: Phrases are case-insensitive.
    :: Phrase that contains only asterisk is not allowed and it will be ignored.

  II. Examples              
    :: Profile Simple.Data.Core.dll
        -asm Simple.Data.Core
    
    :: Profile Simple.Data.Core.dll and Profile Simple.Data.Ado.dll    
        -asm Simple.Data.Core;Simple.Data.Ado

    :: Profile Simple.Data assebmlies
        -asm Simple.Data*

    :: Profile Simple.Data assebmlies and Nop.Data.dll
        -asm Simple.Data*;Nop.Data";
        public override void Execute()
        {
            System.Console.WriteLine(Text);
        }
    }
}