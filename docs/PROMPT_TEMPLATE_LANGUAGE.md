Prompts are the inputs or queries that a user or a program gives to an LLM AI,
in order to elicit a specific response from the model.

Prompts can be natural
language sentences or questions, or code snippets or commands, or any combination
of text or code, depending on the domain and the task.

Prompts can also be nested
or chained, meaning that the output of one prompt can be used as the input of another
prompt, creating more complex and dynamic interactions with the model.

# SK Prompt Template Syntax

The Semantic Kernel prompt template language is a simple and powerful way to
define and compose AI
[functions](GLOSSARY.md)
**using plain text**.
You can use it to create natural language prompts, generate responses, extract
information, **invoke other prompts** or perform any other task that can be
expressed with text.

The language supports three basic features that allow you to (**#1**) include
variables, (**#2**) call external functions, and (**#3**) pass parameters to functions.

You don't need to write any code or import any external libraries, just use the
curly braces `{{...}}` to embed expressions in your prompts.
Semantic Kernel will parse your template and execute the logic behind it.
This way, you can easily integrate AI into your apps with minimal effort and
maximum flexibility.

## Variables

To include a variable value in your text, use the `{{$variableName}}` syntax.
For example, if you have a variable called `name` that holds the user's name,
you can write:

    Hello {{$name}}, welcome to Semantic Kernel!

This will produce a greeting with the user's name.

## Function calls

To call an external function and embed the result in your text, use the
`{{namespace.functionName}}` syntax.
For example, if you have a function called `weather.getForecast` that returns
the weather forecast for a given location, you can write:

    The weather today is {{weather.getForecast}}.

This will produce a sentence with the weather forecast for the default location
stored in the `input` variable.
The `input` variable is set automatically by the kernel when invoking a function.
For instance, the code above is equivalent to:

    The weather today is {{weather.getForecast $input}}.

## Function parameters

To call an external function and pass a parameter to it, use the
`{namespace.functionName $varName}` s