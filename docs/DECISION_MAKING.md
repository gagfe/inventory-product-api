1. Describe a recent technical decision you made with incomplete information. What was missing, and how did you decide to move forward?
I implemented an accounting software service within the application, where the documentation lacked some clear steps and the Product Owner gathered requirements inefficiently, such as:

- What is the workflow if a product's price is changed more than twice?
- The Product Owner poorly defined the requirement by asking for quantities to be sent to the log whenever a change was detected in the textboxes, which caused data loss and   triggered many unnecessary requests.

Solution:

- I implemented a history log to track changes made to the product—not only in cost, but also in its attributes/features.
- I proposed to my manager that we adjust the requirement so data is only submitted when clicking the "Submit" button.

2. When two technical approaches seem equally valid, what criteria do you use to choose one?
I review the requirements to identify which approach best fits the problem.
I compare factors such as scalability, implementation effort/cost, and performance.

3. Share a case where you had to revert or change a technical decision of your own.
When I started implementing a database designed from scratch, deletions were physical (hard deletes) rather than logical. As development progressed and future user stories came up, I decided to change that implementation to a soft delete—simply deactivating records using a boolean (true/false) flag to preserve data auditing and traceability.

4. What did you learn from that experience?
More optimal ways to handle data, and different ways to structure business logic so that structures are clearer, easier to understand, and adaptable to change.