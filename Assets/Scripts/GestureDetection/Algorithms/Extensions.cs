﻿

public static class MyExtensions
{
    public delegate TResult Function<Type, TResult>(Type type);
    public static TResult[] CustomApply<Type,TResult>(this Type[] input, Function<Type, TResult> function)
    {
        TResult[] answers = new TResult[input.Length];
        for (int i = 0; i < answers.Length; i++)
        {
            answers[i] = function(input[i]);
        }
        return answers;
    }
}
