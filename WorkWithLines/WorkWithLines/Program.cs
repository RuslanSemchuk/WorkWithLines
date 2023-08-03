using System;
using System.Collections.Generic;

namespace WorkWithLines;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Enter an arithmetic expression:");
        string input = Console.ReadLine();

        try
        {
            double result = CalculateExpression(input);
            Console.WriteLine("Result: " + result);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }

    static double CalculateExpression(string expression)
    {
        Queue<string> rpnTokens = ConvertToRPN(expression);
        Stack<double> stack = new Stack<double>();

        while (rpnTokens.Count > 0)
        {
            string token = rpnTokens.Dequeue();

            if (double.TryParse(token, out double number))
            {
                stack.Push(number);
            }
            else if (IsOperator(token))
            {
                if (stack.Count < 2)
                    throw new InvalidOperationException("There are not enough operands for the operation: " + token);

                double operand2 = stack.Pop();
                double operand1 = stack.Pop();
                double result = PerformOperation(operand1, operand2, token);
                stack.Push(result);
            }
            else
            {
                throw new InvalidOperationException("Invalid operator or number: " + token);
            }
        }

        if (stack.Count != 1)
            throw new InvalidOperationException("Incorrect expression");

        return stack.Pop();
    }

    static Queue<string> ConvertToRPN(string expression)
    {
        Queue<string> output = new Queue<string>();
        Stack<string> operators = new Stack<string>();

        string[] tokens = expression.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string token in tokens)
        {
            if (double.TryParse(token, out _))
            {
                output.Enqueue(token);
            }
            else if (IsOperator(token))
            {
                while (operators.Count > 0 && IsOperator(operators.Peek()) && GetPrecedence(operators.Peek()) >= GetPrecedence(token))
                {
                    output.Enqueue(operators.Pop());
                }
                operators.Push(token);
            }
            else if (token == "(")
            {
                operators.Push(token);
            }
            else if (token == ")")
            {
                while (operators.Count > 0 && operators.Peek() != "(")
                {
                    output.Enqueue(operators.Pop());
                }

                if (operators.Count == 0 || operators.Peek() != "(")
                    throw new InvalidOperationException("Incorrect brackets");

                operators.Pop();
            }
            else
            {
                throw new InvalidOperationException("Invalid operator or number: " + token);
            }
        }

        while (operators.Count > 0)
        {
            if (operators.Peek() == "(")
                throw new InvalidOperationException("Incorrect brackets");

            output.Enqueue(operators.Pop());
        }

        return output;
    }

    static bool IsOperator(string token)
    {
        return token == "+" || token == "-" || token == "*" || token == "/";
    }

    static int GetPrecedence(string op)
    {
        switch (op)
        {
            case "+":
            case "-":
                return 1;
            case "*":
            case "/":
                return 2;
            default:
                return 0;
        }
    }

    static double PerformOperation(double operand1, double operand2, string op)
    {
        switch (op)
        {
            case "+":
                return operand1 + operand2;
            case "-":
                return operand1 - operand2;
            case "*":
                return operand1 * operand2;
            case "/":
                if (operand2 == 0)
                    throw new DivideByZeroException("Division by zero is prohibited");
                return operand1 / operand2;
            default:
                throw new ArgumentException("Invalid operator: " + op);
        }
    }
}