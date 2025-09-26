# 🔌 API Reference

## BaseForm Class Methods

### Navigation Methods

#### `NavigateToNextForm(FormType currentFormType)`
Navigates to the next form in the sequence.

**Parameters:**
- `currentFormType` (FormType): Current form type

**Example:**
```csharp
NavigateToNextForm(FormType.MainForm); // Goes to Form2
```

#### `NavigateToPreviousForm(FormType currentFormType)`
Navigates to the previous form in the sequence.

**Parameters:**
- `currentFormType` (FormType): Current form type

**Example:**
```csharp
NavigateToPreviousForm(FormType.Form2); // Goes back to MainForm
```

#### `ShowExitConfirmation()`
Displays exit confirmation dialog and closes application if confirmed.

**Example:**
```csharp
ShowExitConfirmation(); // Shows "คุณต้องการปิดโปรแกรมหรือไม่?"
```

### Webcam Methods

#### `StartWebcam()`
Starts webcam with 5-second timeout protection.

**Returns:** void  
**Thread-safe:** Yes

#### `StopWebcam()`
Stops webcam with 4-second timeout protection.

**Returns:** void  
**Thread-safe:** Yes

### FormType Enumeration

```csharp
public enum FormType
{
    MainForm = 1,
    Form2 = 2,
    Form3 = 3,
    Form4 = 4
}
```