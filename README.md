# Sales Forecasting Application

## Overview
This project is a Sales Forecasting Application developed as a proof of concept for the customer Superstore. The application utilizes historical sales data to forecast future sales, allowing the user to query sales data, apply percentage increases to simulate sales growth, and export the forecasted data.

## Technology Stack
- **Programming Language**: C#
- **Framework**: ASP.NET MVC .NET 8
- **Database**: SQL Server
- **Frontend Framework**: ASP.NET MVC
- **Tools**: Visual Studio, SQL Server Management Studio (SSMS)

## Project Structure
The repository contains the following components:
1. **Sales Forecasting Application**: The main project code.
2. **README.md**: This file, explaining the project setup and usage.

## Database
### Tables
The dataset provided includes three tables: Orders, Products, and Returns. These tables have been imported and normalized in SQL Server. Below is a brief description of each table:
- **Orders**: Contains information about customer orders.
- **Products**: Contains information about products sold.
- **Returns**: Contains information about returned products.

### Database Import
The data was imported directly from the provided Excel file into SQL Server using SQL Server Management Studio (SSMS). The database-first approach was used to generate the necessary entity models.

## Features
### User Stories
1. **Query Sales Data**: 
   - Query sales data for a specific year.
   - Display total sales and breakdown by state.
   - Total Sales = Year Sales – Year Returns

2. **Apply Sales Increment**: 
   - Apply a percentage increment to the total sales of a selected year.
   - Display the incremented sales value.
   - Bonus: Apply individual percentages of increase per state.

3. **Export Forecasted Data**: 
   - Download forecasted data to a CSV file with columns: State, Percentage Increase, Sales Value.

### Bonus Features
1. **Sales Charts**: 
   - Display aggregated seeding year sales and forecasted year sales in a chart.
   - Breakdown by state of the seeding year sales and forecasted year sales in a chart.

## Usage

### Query Sales Data
- Navigate to the appropriate section of the application.
- Enter the desired year and submit to view sales data.

### Apply Sales Increment
- Enter the year and the desired percentage increment.
- Submit to view the incremented sales values.

### Export Forecasted Data
- Navigate to the export section.
- Download the CSV file containing the forecasted data.

### View Charts (Bonus)
- Navigate to the charts section.
- View aggregated and state-wise sales charts.

## Explanation of Choices
- **ASP.NET MVC .NET 8**: Chosen for its robustness and familiarity, providing a solid foundation for web applications.
- **Database-First Approach**: Facilitates easier data handling and model generation based on the existing database schema.
- **Direct Data Import**: Using SSMS for data import streamlines the process, making it straightforward to work with the provided dataset.

## Conclusion
This Sales Forecasting Application provides a robust solution for querying and forecasting sales data, offering features to apply percentage increments and export data. The bonus features enhance the application's usability with visual charts for better data analysis.
