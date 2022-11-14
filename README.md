![Ventures Lab](./docs/VL_signature.jpg)

# Ventures Lab Back-end challenge

## Welcome

Hey, welcome, dear candidate! We're thrilled to have you here. This is your back-end challenge. First of all, let's put some very clear statements, so you can be relaxed and confident.

# The General Rules

## What this challenge **IS NOT**:

This challenge intention is to know you. To help us know you quickly and effectively, we want this challenge to be easy, seamlessly and open in its implementation. So, this challenge CAN'T AND WON'T be:

- Long: we hate long challenges where you spent 15 days building a complex solution that will never run.
- Unfair: we hate unfair challenges. We won't ask you to write an HTTP router from scratch. We want to know you, not boring you.
- Full of traps: we hate obscure challenges. You have the right to demand for clarification and you should only write code when you understood what you need to do. On the other hand, it's on you the responsibility to know what you need, to ask for help, to justify and to clarify everything you do. You are responsible for your work. No dead bodies down the road, ok?

## The Requisites

You will write a back-end application. The details of this application will be detailed downwards. But first, these are your first rules/constraints:

- Please, [fork the project](https://support.atlassian.com/bitbucket-cloud/docs/fork-a-repository/) to your own user. In the end, you will submit a [Pull Request (a.k.a. PR)](https://www.atlassian.com/git/tutorials/making-a-pull-request) to our repo with your solution, so we can evaluate your code.
- Time usage: take your time. We all have a busy life. Let us know your schedule. We'll only check your code when you say it's complete and ready to be reviewed (and you provide the PR against our repo, of course).
- We want this application to be written in .NET with C# and ASP.NET Core.
- You can use any UI Library/System you want. Use whatever you feel comfortable and confident. But we will challenge you: why you choose it, what are the pros and cons of your choice. CSS? JSS? SASS? LESS? You decide, you support your decisions ;-) No dead bodies down the road, remember?
- We wan't good code implementation. For that, what we mean is:
  - We want to see a good balance on your code regarding complexity/responsibility/reusability/composability. These are **very important concepts** for us.
  - We want to understand how and why some data are passed through layers and why some of the data are get from some global data layer - in case you think you need one.
  - Is it easy to move/reuse your components in different parts of the application?
- You will write a small application. Quick challenge, remember? But we will evaluate your solution **pretending you're building a big one**. So, we won't accept excuses like "Oh, this was a small challenge, so I just used a single local `repository` with a `allMyData` object inside". No, that's not fair. We don't want spend your time writing hundreds of classes, but we **do want to see how you think when building a real world .NET application**. Please, mind this concept when writing your solution.
- Unit tests: these are important. You do NOT need to write tests for all parts of your application. BUT we'll judge what you tested and what you didn't. Tests are valuable, but they're also hard to keep. So, finding a balance of time and check what's more important to test on your solution is also on the line. And we will ask you about your decisions on this subject.
- GIT: we love good Git commit description. If they follow some standard format, this is even better. Also, we want to understand the progress of your work through your commits, so avoid to send just a single commit with "that's all" message on it.

## The bonuses

We're a gambling company. We LOVE bonuses. Our users love bonuses even more. So, here are your bonuses:

- Bonus for excellent OOP usage. We'll love to read and understand your types, interfaces, etc.
- Bonus if you use an HTTP/Cache managed layer, like Redis. But, if you're not familiar with it, you won't loose points. Use it if you're confident with these concepts.
- Bonus for wise form input data control and validations. This have a great impact for our eyes.
- Bonus for good code metrics, rules and code format (like [code maid](https://www.codemaid.net/)). We love well formatted and linted code. Even more when it's automatically linted on `git push` events.
- Bonus for Database with docker or in memory
- Bonus for React in the frontend
- Bonus for Rest API implementation
- Feel free to use any ORM you like, ADO.NET, EF, Dapper

> Remember: these are bonus points. Keep your focus and deliver the important parts first.

## In the end, it's just C#. Right?

You'll build a C# solution. And we value great code. Good, clear, concise, and testable code is what we want in the end. Tips? Sure!

- Avoid unnecessary `else` statements using early returns
- Avoid switch cases using Object Literals strategy.
- Avoid global loops like `for (var i = 0; i < length; i++)`. We already have best options for those, right?
- Avoid useless imports and unused variables.

## The Application

Build an application to manage a user's tasks.

- Create, delete and edit a task
- List of today's and upcoming tasks, grouped by date and time

Task must contain the data below:

- User
- Date
- Start and end time
- Subject
- Description

The rules for the submission is:

- Instructions for setting up and running the project must be provided in the README.
- The solution must be compiling successfully.
- After previous items, the solution must run without errors.

## Final Notes

If you want, you can add a "SOLUTION.md" file, to add notes to your solution. Instructions on how to build and run your solution are very welcomed.

Optional: any other information you want to share/consider is also welcomed. Feel free to highlight the strong points of your code or maybe improvements you could do in case you had more time or if it were a real application. This is optional, don't spend too much time on it or don't do it at all.

If you need help, please reach out! Talk to our team by mail or any other contact you may have. We're here to help and clarify your path. Just like we would in our real projects. Let's create some beautiful software together.
