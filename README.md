# Logicality lib.
This is a set of useful basic constructs for C# development. Includes **Activatalbe** and **Assignments**. That constructs are independent and can be used separately from each other.

## Activatable
That contains an interface and extensions for entities that have active and inactive states and can be in transition states for some time - from active to inactive and back. An activatable entity has a read-only field indicating the current state and event signaling a change in state.

See <a href="https://raw.githack.com/vcow/lib-logicality/master/docs/html/namespaces.html">documentation</a> for details.

### Applying
You can download and install <code>logicality.unitypackage</code> from this repository or add as dependency from **Github** or directly include them in your Git project as **subtree**.

#### Github
Go to the <code>manifest.json</code> and in the section <code>dependencies</code> add next dependency:
```
  "dependencies": {
    "vcow.base.activatable": "https://github.com/vcow/lib-logicality.git?path=/Assets/Scripts/Base/Activatable/#3.0.0",
    ...
  }
```

#### Subtree
From the root or your Git project launch next Git command:
```
git subtree add --prefix Assets/Scripts/Base/Activatable --squash git@github.com:vcow/lib-logicality.git activatable-1.0.0
```
That adds Activatable from this repository to your project as a subtree at the location specified in the <code>--prefix</code> section relative to the root of your project.

## Assignment
This is a kind of tasks that is launched, reports its completion, and can be organized into various sequences. Sequences are created using a **queue** and a **concurrent**, which are themselves assignments. Assignments in the **queue** are executed sequentially in the order in which they where added. Assignments in the **concurrent** are executed simultaneously in one shared or several threads.

See <a href="https://raw.githack.com/vcow/lib-logicality/master/docs/html/namespaces.html">documentation</a> for details.

### Applying
You can download and install <code>logicality.unitypackage</code> from this repository or add as dependency from **Github** or directly include them in your Git project as **subtree**.

#### Github
Go to the <code>manifest.json</code> and in the section <code>dependencies</code> add next dependency:
```
  "dependencies": {
    "vcow.base.assignments": "https://github.com/vcow/lib-logicality.git?path=/Assets/Scripts/Base/Assignments#3.0.0",
    ...
  }
```

#### Subtree
From the root or your Git project launch next Git command:
```
git subtree add --prefix Assets/Scripts/Base/Assignments --squash git@github.com:vcow/lib-logicality.git assignments-1.0.0
```
That adds Assignments from this repository to your project as a subtree at the location specified in the <code>--prefix</code> section relative to the root of your project.
