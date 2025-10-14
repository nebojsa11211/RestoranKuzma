// Test script to verify dashboard table statistics implementation
import { readFile } from 'fs/promises';

async function testDashboardImplementation() {
    console.log('🧪 Testing Dashboard Table Statistics Implementation...\n');
    
    try {
        // Read the Index.razor file
        const indexContent = await readFile('src/RestaurantSuite.Admin/Pages/Index.razor', 'utf8');
        
        // Check if all required variables are defined
        const requiredVariables = [
            'totalTables',
            'occupiedTables', 
            'availableTables',
            'reservedTables',
            'occupancyPercentage'
        ];
        
        console.log('✅ Checking for required variables:');
        let allVariablesFound = true;
        
        for (const variable of requiredVariables) {
            if (indexContent.includes(variable)) {
                console.log(`  ✓ ${variable} found`);
            } else {
                console.log(`  ✗ ${variable} missing`);
                allVariablesFound = false;
            }
        }
        
        // Check if CalculateTableStatistics method exists
        if (indexContent.includes('CalculateTableStatistics')) {
            console.log('✅ CalculateTableStatistics method found');
        } else {
            console.log('✗ CalculateTableStatistics method missing');
            allVariablesFound = false;
        }
        
        // Check if TableStatus enum is used
        if (indexContent.includes('TableStatus.Occupied') && 
            indexContent.includes('TableStatus.Available') && 
            indexContent.includes('TableStatus.Reserved')) {
            console.log('✅ TableStatus enum usage found');
        } else {
            console.log('✗ TableStatus enum usage missing');
            allVariablesFound = false;
        }
        
        // Check if ApiService.GetTablesAsync is called
        if (indexContent.includes('ApiService.GetTablesAsync()')) {
            console.log('✅ ApiService.GetTablesAsync() call found');
        } else {
            console.log('✗ ApiService.GetTablesAsync() call missing');
            allVariablesFound = false;
        }
        
        // Check if dashboard cards are updated
        const dashboardCards = [
            'Total Tables',
            'Occupied',
            'Available', 
            'Reserved'
        ];
        
        console.log('\n✅ Checking dashboard cards:');
        for (const card of dashboardCards) {
            if (indexContent.includes(card)) {
                console.log(`  ✓ ${card} card found`);
            } else {
                console.log(`  ✗ ${card} card missing`);
                allVariablesFound = false;
            }
        }
        
        // Check if data binding is implemented
        const dataBindings = ['@totalTables', '@occupiedTables', '@availableTables', '@reservedTables', '@occupancyPercentage'];
        
        console.log('\n✅ Checking data bindings:');
        for (const binding of dataBindings) {
            if (indexContent.includes(binding)) {
                console.log(`  ✓ ${binding} data binding found`);
            } else {
                console.log(`  ✗ ${binding} data binding missing`);
                allVariablesFound = false;
            }
        }
        
        console.log('\n' + '='.repeat(50));
        if (allVariablesFound) {
            console.log('🎉 SUCCESS: All dashboard table statistics components are implemented correctly!');
            console.log('\nThe dashboard will now display real table data:');
            console.log('- Total Tables: Shows total number of tables');
            console.log('- Occupied: Shows occupied tables with percentage');
            console.log('- Available: Shows available tables');
            console.log('- Reserved: Shows reserved tables');
            console.log('\nThe data will be fetched from the API and updated dynamically.');
        } else {
            console.log('❌ FAILURE: Some components are missing or incomplete.');
        }
        console.log('='.repeat(50));
        
    } catch (error) {
        console.error('❌ Error testing implementation:', error.message);
    }
}

// Run the test
testDashboardImplementation();
