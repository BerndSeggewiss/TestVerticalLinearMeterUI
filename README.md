# TestVerticalLinearMeterUI
Read analog voltage from arduino and display it in UI

## Description

A real-time voltage monitoring application that reads analog voltage from an Arduino board and 
displays the values using an linear meter UI from ChartDirector.

## What it does

- **Reads** analog voltage from Arduino pin A0 (0-5V range)
- **Transmits** data via serial communication (9600 baud)
- **Displays** live voltage readings on a color-coded linear meter gauge
- **Updates** in real-time with smooth visual feedback

## 🛠️ Technology

- **Framework**: .NET 9.0 / WPF
- **Language**: C# 13.0
- **Visualization**: [ChartDirector for .NET](https://www.advsofteng.com/)
- **Serial Communication**: System.IO.Ports
