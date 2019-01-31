# CommentStripper
Removes XML comments in the XML doc file, for non-public members using the compiled dll.

Current status
[![Build status](https://ci.appveyor.com/api/projects/status/4ik906q7g01m2q44?svg=true)](https://ci.appveyor.com/project/Meberem/commentstripper)

By adding this package to your project, any private XML doc comments (three slashes - /// ) will be removed from the generated XML doc file. This will not add anything to the project output, you can verify that the nuget is a developmentDependency, it runs an executable as a post build step.

## Usage

Run:
```
Install-Package CommentStripper
```

or run:
```
dotnet add package CommentStripper
```

or add:
```
<PackageReference Include="CommentStripper" Version="0.0.1">
    <PrivateAssets>all</PrivateAssets>
    <IncludeAssets>runtime; build; native; contentfiles; analyzers</IncludeAssets>
</PackageReference>
```

## Sample

Code that looks like: ![xml documented code](https://raw.githubusercontent.com/Meberem/CommentStripper/build-tooling/images/code-sample.PNG) generating ![xml doc with private member comments](https://raw.githubusercontent.com/Meberem/CommentStripper/build-tooling/images/framework-application-pre.PNG)

Will no longer have the comments from the private members ![xml doc without the private member comments](https://raw.githubusercontent.com/Meberem/CommentStripper/build-tooling/images/framework-application-post.PNG)