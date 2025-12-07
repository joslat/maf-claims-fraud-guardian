# Script to copy remaining model files from ClaimsCoreMcp to ClaimsCore.Common
# Updates namespace from ClaimsCoreMcp.Models to ClaimsCore.Common.Models

$workflowFiles = @(
    "IntakeDecision.cs",
    "ProcessedClaim.cs",
    "ClaimReadinessStatus.cs"
)

$fraudFiles = @(
    "DataReviewResult.cs",
    "OSINTFinding.cs",
    "UserHistoryFinding.cs",
    "TransactionFraudFinding.cs",
    "FraudDecision.cs",
    "FraudAnalysisState.cs",
    "MarketplaceCheck.cs",
    "TransactionRiskProfile.cs"
)

# Copy Workflow models
foreach ($file in $workflowFiles) {
    $sourcePath = "src/ClaimsCoreMcp/Models/$file"
    $destPath = "src/ClaimsCore.Common/Models/Workflow/$file"
    
    if (Test-Path $sourcePath) {
        $content = Get-Content $sourcePath -Raw
        $content = $content -replace "namespace ClaimsCoreMcp\.Models;", "namespace ClaimsCore.Common.Models;"
        
        # Ensure directory exists
        $destDir = Split-Path $destPath -Parent
        if (-not (Test-Path $destDir)) {
            New-Item -ItemType Directory -Path $destDir -Force | Out-Null
        }
        
        Set-Content -Path $destPath -Value $content
        Write-Host "? Copied $file to Workflow folder" -ForegroundColor Green
    }
}

# Copy Fraud models
foreach ($file in $fraudFiles) {
    $sourcePath = "src/ClaimsCoreMcp/Models/$file"
    $destPath = "src/ClaimsCore.Common/Models/Fraud/$file"
    
    if (Test-Path $sourcePath) {
        $content = Get-Content $sourcePath -Raw
        $content = $content -replace "namespace ClaimsCoreMcp\.Models;", "namespace ClaimsCore.Common.Models;"
        
        # Ensure directory exists
        $destDir = Split-Path $destPath -Parent
        if (-not (Test-Path $destDir)) {
            New-Item -ItemType Directory -Path $destDir -Force | Out-Null
        }
        
        Set-Content -Path $destPath -Value $content
        Write-Host "? Copied $file to Fraud folder" -ForegroundColor Green
    }
}

Write-Host "`n?? All models copied successfully!" -ForegroundColor Cyan
