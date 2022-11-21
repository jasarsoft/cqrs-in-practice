## CQRS in Practice
#### Course by Vladimir Khorikov

There are a lot of misconceptions around the CQRS pattern. This course is an in-depth guideline into every concern or implementation question you've ever had about CQRS.

#### Course Overview<br>
The CQRS pattern has become quite well known in the sphere of Domain-Driven Design; however, there are still a lot of misconceptions around this pattern, especially when it comes to applying it in the real-world software projects. Should you use it with event sourcing? Should you always have separate databases for reads and writes? How to implement command handlers using the modern frameworks such as ASP. NET Core. The course will answer all these questions and more. Some of the major topics that we will cover include: refactoring towards task-based interface and away from the crowd-based thinking, implementing command and query handler decorators, extracting separate data storage for reads, and common best practices and misconceptions around CQRS. By the end of this course, you will know everything needed to start implementing the CQRS pattern in your own projects. Before beginning this course, you should be familiar with the C# programming language. I hope you will join me on this journey to learn CQRS here at Pluralsight.

#### Introduction
If you are familiar with Domain-Driven Design, you've most likely heard about CQRS, which stands for Command-Query Responsibility Segregation. In fact, this pattern has become almost as well-known as the concept of Domain-Driven Design itself; however, there are still a lot of misconceptions around this pattern, especially when it comes to applying it in the real-world software projects. Should you use it with event sourcing? Should you always have separate databases for reads and writes? Should you make the synchronization between reads and writes asynchronous? And so on and so forth. This course will answer all of these questions and more. You will learn exactly what CQRS is, the principles behind it, and the benefits it can provide for your project. You will also learn about all the common misconceptions and anti-patterns around it. You will see a detailed, step-by-step process of implementing this pattern in practice. The sample project we'll be working on is close to what you can see in the real world, and I will explain each step on the way to CQRS in great detail. Here's a quick outline of this course. In the first module, we will talk about what CQRS is, its origin, and the benefits it can provide for your project. In the next module, I will introduce an online student management system implemented without the CQRS pattern in mind. You will see firsthand how the CRUD-based API reduces the quality of one's code base and damages the user experience. In the third module, we will start the refactoring. We will segregate commands from queries by introducing two models where previously there was only one. You will see how it allows us to offload the read operations from the domain model and thus make this model simpler. Next, you will learn what a CRUD-based interface is and how to refactor it towards a task-based one. In the fifth module, we will simplify the read model. We will achieve that by bypassing the domain model and the ORM when reading data from the database. This will allow us to optimize the performance of reads in the application. In the next module, we will look at MediatR, an open-source library that was built specifically with CQRS in mind and perfectly fits this pattern. In the seventh module, we are going to introduce a separate database for queries, and in Module 8, implement synchronization between the two. Finally, in the last module, we will talk about CQRS best practices and misconceptions. You will learn about common questions people ask when starting out with CQRS, such as, for example, the distinction from event sourcing. During the course, you will see a lot of refactoring techniques, which I'll explain in detail as we go through them. For this course, you will need a basic knowledge of what Domain-Driven Design is. I recommend my Domain-Driven Design in Practice course to obtain this knowledge.

#### CQRS and Its Origins
CQRS stands for Command-Query Responsibility Segregation. The idea behind this pattern is extremely simple. Instead of having one unified model, you need to introduce two: one for reads and the other one for writes, and that's basically it. Despite its simplicity, however, this simple guideline leads to some significant benefits. We will cover all of them in the subsequent modules. For now, let's elaborate on this basic idea, and talk about the origin of CQRS. CQRS was introduced by Greg Young back in 2010. Here's the book about CQRS he wrote in the same year. Greg, himself, based this idea on the command-query separation principle coined by Bertrand Meyer. Command-query separation principle, CQS for short, states that every method should either be a command that performs an action, or a query that returns data to the caller, but not both. In other words, asking a question should not change the answer. More formally, methods should return a value only if they are referentially transparent and don't incur any side effects, such as, for example, mutating the state of an object, changing a file in the file system, and so on. To follow this principle, you need to make sure that if a method changes some piece of state, this method should always be of type void, otherwise, it should return something. This allows you to increase the readability of your code base. Now you can tell the method's purpose just by looking at its signature. No need to dive into its implementation details. Note that it is not always possible to follow the command-query separation principle and there almost always will be situations where it would make more sense for a method to both have a side effect and return something. An example here is Stack. Its Pop method removes the element pushed into the stack last and returns it to the caller. This method violates the CQS principle, but at the same time, it doesn't make a lot of sense to separate those responsibilities into two different functions. Other examples include situations where the result of a query can become stale quickly, and so you have to join the query with the command. In this case, you both perform the operation and return the result of it. For example, here, on the left, you can see two methods, one for writing to a file, and the other one for ensuring that this file exists. The idea here is that before calling this method, the client code needs to make sure the file exists by using this one. The FileExists method is a query here. It returns a Boolean value and doesn't mutate the file, and WriteToFile is a command. It changes the file and its return type is void. So, these two methods follow the CQS principle. However, there's a problem with this code. The result of the query can become stale by the time the client code invokes the command. There could be some other process intervening right between these two calls, and it can delete the file after the query is called, but before we invoke the command, and so to avoid this problem, we have to violate the command-query separation principle and come up with a new version of the method, like this one. As you can see, instead of checking for the file existence, it tries to update it, and if there's no such file, it gets an exception, catches it, and returns a failed result. The operation is atomic now. That's how we avoid information staleness. The downside here is that this method no longer follows the CQS principle. Other examples where the command-query separation principle is not applicable involve multi-threaded environments where you also need to ensure that the operation is atomic. However, it's still a good idea to make the CQS principle your default choice, and depart from it only in exceptional cases, like those I described above. So what's the relation between CQS and CQRS? CQRS takes this same idea and extends it to a higher level. Instead of methods like in CQS, CQRS focuses on the model and classes in that model, and then applies the same principles to them. Just like CQS encourages you to split a method into two, a command and a query, CQRS encourages you to untangle a single, unified domain model and create two models: one for handling commands or writes, and the other one for handling queries, reads. Like I said, the principle is extremely simple. However, it entails very interesting consequences. So let's discuss them next.

#### Why CQRS?
Alright, so CQRS is about splitting a single model into two; one for reads and the other one for writes, but of course, this is not the end goal in and of itself. So what is it then? What are the benefits the CQRS pattern provides? First of all, it's scalability. If you look at a typical enterprise level application, you may notice that among all operations with this application, among all those create, read, update, and delete operations, the one that is used the most is usually read. There are disproportionately more reads than writes in a typical system, and so it's important to be able to scale them independently from each other. For example, you can host the command side on a single server, but create a cluster of 10 servers for the queries. Because the processing of commands and queries is fundamentally asymmetrical, scaling these services asymmetrically makes a lot of sense, too. Secondly, it's performance. This one is related to scalability, but it's not the same thing. Even if you decide to host reads and writes on the same server, you can still apply optimization techniques that wouldn't be possible with a single unified model. For example, just having a separate set of APIs for queries allows you to set up a cache for that specific part of the application. It also allows you to use database-specific features and hand-crafted, highly sophisticated SQL for reading data from the database without looking back at the command side of the application where you probably use some kind of ORM, but probably the most significant benefit here is simplicity. The command side and the query side have drastically different needs, and trying to come up with a unified model for these needs is like trying to fit a square peg in a round hole. The result always turns out to be a convoluted and over-complicated model that handles neither of these two parts well. And often, the very act of admitting that there are two different use cases involved allows you to look at your application in a new light. By making the difference between them explicit and introducing two models instead of just one, you can offload a lot of complexity from your code base. Now all of the sudden, you don't need to worry about handling two completely different use cases with the same code. You can focus on each of them independently and introduce a separate solution that makes the most sense in each particular case. You can view this as the single responsibility principle applied at the architectural level. In the end, you get two models, each of which does only one thing, and does it well. So, to summarize, we can say that CQRS is about optimizing decisions for different situations. You can choose different levels of consistency, different database normal forms, and even different databases themselves for the command and query sides, all because you are able to think of commands and queries and approach them independently. You will see all these benefits in action when we'll be working on the sample project.

#### CQRS in the Real World
You might not realize it, but you probably have already employed the CQRS pattern in one form or the other in the past. Let's look at some examples from real-world projects. If you ever used Entity Framework or NHibernate for writing data to the database, and raw SQL with plain ADO. NET for reading it back, that was CQRS right there. You probably thought at the moment that the necessity to drop the ORM and resort to the bare SQL for reads is just that, the necessary evil, a trade-off you have to make in order to comply with the performance requirements, but, no. This is a perfectly legitimate pattern, CQRS that is. Also, if you ever created database views optimized for specific read use cases, that was a form of CQRS as well. Another common example is ElasticSearch or any other full-text search engine. It works by indexing data, usually from a relational database, and providing rich capabilities to query it. That's exactly what CQRS is about. You have one model for writes and a completely separate model for reads, except that in this particular case, you don't build that second model yourself, but leverage an already-existing software.

#### Summary
In this module, you learned that Command Query Responsibility Segregation is a pattern originating from the command-query separation principle. CQRS extends CQS to the architectural level. Just like CQS encourages you to split a method into two methods, a query and a command, CQRS encourages you to untangle a single, unified domain model and create two models: one for handling commands, and the other one for handling queries. CQRS allows us to make different decisions for reads and writes, which in turn brings three benefits: scalability, performance, and the biggest one, simplicity. You can view CQRS as the single responsibility principle applied at the architectural level. In the end, you get two models, each of which does only one thing, and does it really well. We also discussed examples of applying the CQRS pattern in the real world. ElasticSearch and database views are among them. In the next module, we will look at a sample project that's implemented without the CQRS pattern in mind. We will analyze it, discuss its drawbacks, and then start making steps towards implementing CQRS.