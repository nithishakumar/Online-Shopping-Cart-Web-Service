# Online-Shopping-Cart-Web-Service

An ASMX web service (built with 3-tier architecture) that fetches data of chosen items from an SQL database and implements an 
online shopping cart ordering system with security procedures. Users can also upload a Know Your Customer Form (pdf file) whose 
binary data is stored in the SQL database. A test ASPX web page client for the web service is also included. All SOAP messages 
sent by the web service in the XML format, including SOAP fault messages, are formatted in an easily accessible manner with
headers that indicate the status of the client's request (SUCCESS/FAILURE) and nodes/attributes for each section of data in the
response.

This web service is a prototype of one for an automobile company's upcoming mobile app that would enable customers to inquire and
order automobile spare parts. It helps the app communicate with the company's database by fetching data as per the mobile app's 
requests. In all web methods, user authentication is performed and all exceptions possible are handled. Apart from a generic error
message, error messages specific to some exceptions - like the client’s inability to connect to the web service, absence of a 
specific part number, invalid quantity, etc. - are caught from the web service and displayed on the test web page. The main 
functions of this web service include:

1. Fetching part details from the SQL database
2. Selecting and adding parts to a cart
3. Displaying existing items in the cart (data of parts added to the cart is retained even from previous sessions)
4. Edit existing parts in the cart (change the quantity and remarks provided for each item)
5. Upload a Know Your Customer form (pdf file), view the uploaded file, and retrieve it later from the SQL database
   for customer verification
   
The data access layer of the web service is split into two separate entity classes that handle transactional activities
(adding/modifying/deleting parts from the cart, uploading a file) and non-transactional activities (viewing uploaded files,
viewing the cart, fetching available parts) respectively. All transactions are rolled-back when an exception occurs. 
Exceptions that occur while rolling back transactions are also handled.

The SQL stored procedure "DTPartsSelection" can be replaced by a less complex procedure that just fetches data from a single table. 
A sample for the same is provided. Properties of most tables used in SQL queries are also provided. This web service currently
only authorizes one user but it can easily be optimized to authorize more users by storing user data in the SQL database and 
following the web service's existing authentication procedures.



