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

function Extract-Views {
    param(
        [Parameter(Mandatory=$true, HelpMessage = "The name of the assembly that contains embedded views.")]
        [string]$AssemblyName, 
        
        [Parameter(HelpMessage = "The directory relative to the Views directory where you want to save the views. e.g. if 'Foo\Bar' views are saved in 'Views\Foo\Bar'")]
        [string]$ViewsDirectory = $null
    )
    
    $project = Get-Project
    $isWebsite = Project-Is-Website $project
    $projectName = $project.Name
    $assemblyRef = $project.Object.References | where {(AssemblyName-From-Ref $_ $isWebsite) -eq $AssemblyName}
    
    if (-not $assemblyRef) {
        Write-Error "Couldn't find $AssemblyName reference in $projectName."
        return
    }
    
    $assemblyPath = if ($isWebsite) { $assemblyRef.FullPath } else { $assemblyRef.Path }
    $assembly = [Reflection.Assembly]::LoadFrom($assemblyPath)
    $viewResourceNames = $assembly.GetManifestResourceNames() | 
        where {[Text.RegularExpressions.Regex]::IsMatch($_, "^$AssemblyName\.Views\.", [Text.RegularExpressions.RegexOptions]::IgnoreCase)}
    
    if ($viewResourceNames -eq $null `
        -or $viewResourceNames.Length -eq 0) {
        Write-Error "$AssemblyName doesn't have embedded views."
        return
    }
       
    $projectPath = ($project.Properties | where {$_.Name -eq "FullPath"}).Value
    $overwriteAll = $false
    $extracted = 0;
    
    write "Extracting..."
    
    foreach($res in $viewResourceNames) {
        
        $resParts = $res.Split(".")
        $viewFileName = [string]::Join(".", $resParts[($resParts.Length - 2)..($resParts.Length - 1)])
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
            $fileStream = [IO.File]::Create($viewPath)
            $resStream.CopyTo($fileStream)
            $fileStream.Dispose()
            $resStream.Dispose()
            
            $extracted++
            
            if (-not $isWebsite) {
                [void]$project.ProjectItems.AddFromFile($viewPath)
            }
        }
    }
    
    if ($extracted `
        -and $isWebsite) {
        
        # Focus on Solution Explorer
        #$dte.Windows.Item("{3AE79031-E1BC-11D0-8F78-00A0C9110057}").Activate()
        
        try {
            # Refresh
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
        'AssemblyName' = { (Get-Project).Object.References | % {AssemblyName-From-Ref $_ (Project-Is-Website (Get-Project))} | sort $_ }
    }
}

Export-ModuleMember Extract-Views
