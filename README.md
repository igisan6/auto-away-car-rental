# Auto Away
Car rental app designed to work for both customers and employees. The app emulates systems used by car rental companies by allowing users to view vehicles, create bookings, return vehicles, modify vehicle entries, and view reports. Written in C#, with the UI designed in Visual Studio 2019. The database uses Microsoft SQL server, and is managed through SQL Server Management Studio.

![Alt text](https://lh5.googleusercontent.com/wGFfElyjUAhuGkUPXivHFI_8XdwaczaUQx7BM-tewOlD--Ya2u2wuCPiUmLhYUTHR3BGRNXQ6oDDvw29eGnQ5CoCZvWK7Ilhwc8MF2rp5DganfGeAgJiXDqlhMld9A)

Employees can browse available vehicles based on criteria such as make, model, location, price, and date range. Employees can also create bookings for customers, as well as process vehicles that have been returned. When a booking is created, that vehicle is removed from the available list of vehicles.

![Alt text](https://lh4.googleusercontent.com/NIjr-BoDKABYVWAZmmPMuh8uUh9YtC-ndQ3P7PTnQaBee-VzXqcnkeLK8D0Dn2gz9f5DLWYp_qZ7DMeTM1YnOBCsFg4_k7_6hkA1wpEYODRT6Y087t63NUNTSCEORw)

Employees can create, update, and modify vehicle entries. An employee can search by Car ID, and the resulting model, year, class, and branch will be displayed.

![Alt text](https://lh6.googleusercontent.com/DKSOHHXEdzgcHGVSHetfiXNKz4bR9g8yXWIgEM3boUy6YtGvqghm3dwqft-5eoBZ27MMG4DRDuSnH1wCs7Rjx6oihr8D21Vc-Rvb3prNR6gBXQrfjxuxRHJYc3CyKQ)

Employees can access different categories of reports — cars, customers, branches, bookings, and custom reports. Each category can display reports based on specific criteria and date range. For the custom reports, an employee familiar with SQL can directly access the database using queries.

![Alt text](https://lh6.googleusercontent.com/LATRwIydHLYxTi8j5UGKdtO59Ydn0N4DJdhhgLGnfECe9kHQ9wH47FEBjwiqRnuoTDR3GGQ8UHnrpqURMopBDKtSGng_mVOWaEVTzIvnDfJqOmT434JLr9Di8nndJw)

When an employee processes a vehicle return, the app calculates the rental cost and displays a breakdown. It is calculated by the cost (either daily, weekly, or monthly) multiplied by the number of days booked. If the vehicle is returned late, the customer incurs late fees. If the vehicle is returned at a different branch, the customer is charged a different branch fee. When the car has been returned, it shows up in the system as an available vehicle.
