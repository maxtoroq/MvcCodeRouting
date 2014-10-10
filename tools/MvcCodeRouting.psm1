# Copyright 2012 Max Toro Q.
#
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
#     http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.

$ErrorActionPreference = "Stop"

function Extract-Views {
    <#
     .SYNOPSIS
      Copies assembly-embedded views to your project.
      
     .DESCRIPTION
      Copies assembly-embedded views to your project.
      
     .PARAMETER AssemblyName
      Specifies the assembly that contains embedded views.
      
     .PARAMETER ViewsDirectory
      Specifies the directory relative to the 'Views' directory where you want to save the views. e.g. if 'Foo\Bar' views are saved in 'Views\Foo\Bar'. If omitted, views are saved directly in 'Views'.
      
     .PARAMETER ProjectName
      Specifies the project to use as context. If omitted, the default project is chosen.
      
     .PARAMETER Culture
      Specifies the culture of the satellite assembly to extract views from. If omitted, views are extracted from the main assembly.
      
     .EXAMPLE
      PM> Extract-Views MvcAccount Account
    #>
    param(
        [Parameter(Mandatory=$true, Position=0)]
        [string]$AssemblyName,
        
        [Parameter(Position=1)]
        [string]$ViewsDirectory = $null,
        
        [Parameter(Position=2)]
        [string]$ProjectName = $null,
        
        [Parameter()]
        [string]$Culture = $null
    )
    
    $project = $null
    
    if ($ProjectName) {
        $project = Get-Project $ProjectName
    } else {
        $project = Get-Project
        $ProjectName = $project.Name
    }
    
    if ($project -eq $null) {
        throw "Couldn't find project $ProjectName."
    }
    
    $isWebsite = Project-Is-Website $project
    $assemblyRef = $project.Object.References | where {(AssemblyName-From-Ref $_ $isWebsite) -eq $AssemblyName}
    
    if (-not $assemblyRef) {
        throw "Couldn't find $AssemblyName reference in $ProjectName."
    }
    
    $cult = $null
    
    if ($Culture) {
        $cult = [Globalization.CultureInfo]::GetCultureInfo($Culture)
    }    
    
    
    $assemblyPath = if ($isWebsite) { $assemblyRef.FullPath } else { $assemblyRef.Path }
    
    if ($cult) {
        $assemblyPath = [IO.Path]::Combine([IO.Path]::GetDirectoryName($assemblyPath), $cult.Name, [IO.Path]::GetFileNameWithoutExtension($assemblyPath)) + ".resources.dll"
    }
    
    $assembly = [Reflection.Assembly]::Load([IO.File]::ReadAllBytes($assemblyPath))
    
    $viewResourceNames = $assembly.GetManifestResourceNames() | 
        where {[Text.RegularExpressions.Regex]::IsMatch($_, "^$AssemblyName\.Views\.", [Text.RegularExpressions.RegexOptions]::IgnoreCase)}
    
    if ($viewResourceNames -eq $null `
        -or $viewResourceNames.Length -eq 0) {
        
        throw "$AssemblyName doesn't have embedded views."
    }
       
    $projectPath = ($project.Properties | where {$_.Name -eq "FullPath"}).Value
    $overwriteAll = $false
    $extracted = 0;
    
    write "Extracting..."
    
    foreach($res in $viewResourceNames) {
        
        $resParts = $res.Split(".")
        $viewFileName = [string]::Join(".", $resParts[($resParts.Length - 2)..($resParts.Length - 1)])
        
        if ($cult) {
            $viewFileName = [IO.Path]::GetFileNameWithoutExtension($viewFileName) + "." + $cult.Name + [IO.Path]::GetExtension($viewFileName)
        }
        
        $viewDirParts = New-Object Collections.ArrayList
        $viewDirParts.AddRange($resParts[($AssemblyName.Split('.').Length)..($resParts.Length - 3)])
        
        if ($ViewsDirectory) {
            $viewDirParts.Insert(1, $ViewsDirectory)
        }
        
        $viewDir = [string]::Join([IO.Path]::DirectorySeparatorChar, $viewDirParts.ToArray())
        $viewRelativePath = [IO.Path]::Combine($viewDir, $viewFileName)
        $viewDirPath = [IO.Path]::Combine($projectPath, $viewDir)
        $viewPath = [IO.Path]::Combine($viewDirPath, $viewFileName)
        $createFile = $true
        
        if (-not [IO.Directory]::Exists($viewDirPath)) {
            [void][IO.Directory]::CreateDirectory($viewDirPath)
        }
        
        if ([IO.File]::Exists($viewPath) `
            -and -not $overwriteAll) {
            
            switch (Prompt-Choices -Choices ("&Yes", "Yes to &All", "&No") `
                -Title $null `
                -Message "$viewRelativePath already exists in your project. Overwrite?" `
                -Default 2) {
                
                1 { $overwriteAll = $true }
                2 { $createFile = $false }
            }
        }
        
        if ($createFile) {
            
            $resStream = $assembly.GetManifestResourceStream($res)
            
            try {
            
                $fileStream = [IO.File]::Create($viewPath)
            
                try {
                
                    $resStream.CopyTo($fileStream)
                
                } finally {
                    $fileStream.Dispose()
                }
            
            } finally {
                $resStream.Dispose()
            }
            
            $extracted++
            
            if (-not $isWebsite) {
                [void]$project.ProjectItems.AddFromFile($viewPath)
            }
        }
    }
    
    if ($extracted `
        -and $isWebsite) {
        
        try {
            # Refresh Solution Explorer
            $dte.Commands.Raise("{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}", 222, [ref]$null, [ref]$null)
        } catch { }
    }
    
    write "$extracted file(s) extracted."
}

function Project-Is-Website($project) {
    return $project.Type -eq "Web Site"
}

function AssemblyName-From-Ref($assemblyRef, $isWebsite) {
    if ($isWebsite) { 
        return $assemblyRef.StrongName.Substring(0, $assemblyRef.StrongName.IndexOf(",")) 
    } else { 
        return $assemblyRef.Identity 
    }
}

function Prompt-Choices($Choices=("&Yes", "&No"), [string]$Title="Confirm", [string]$Message="Are you sure?", [int]$Default=0) {
	
    $choicesArr = [Management.Automation.Host.ChoiceDescription[]] `
        ($Choices | % {New-Object Management.Automation.Host.ChoiceDescription $_})
	return $host.ui.PromptForChoice($Title, $Message, $choicesArr, $Default)
}

if (-not (Get-Command Get-Project -ErrorAction SilentlyContinue)) { 
    throw "Couldn't find the Get-Project command. Please import this module from the Package Manager Console in Visual Studio."
}

if ((Get-Command Register-TabExpansion -ErrorAction SilentlyContinue)) {
       
    Register-TabExpansion 'Extract-Views' @{
        'AssemblyName' = { 
            param($Context)
            
            $project = $null
            
            if ($Context.ProjectName) {
                $project = Get-Project $Context.ProjectName
            } else {
                $project = Get-Project
            }
                       
            $project.Object.References | % {AssemblyName-From-Ref $_ (Project-Is-Website $project)} | sort $_ 
        };
        'ProjectName' = { $dte.Solution | % {$_.ProjectName} }
    }
}

Export-ModuleMember Extract-Views
