
/**
 * Test script for abp.js
 * This script invokes various functions defined in abp.js to verify their functionality.
 */

(function() {
    console.log("--- Starting abp.js tests ---");

    // 1. Application Paths
    console.log("Testing Application Paths:");
    console.log("abp.appPath:", abp.appPath);
    console.log("abp.toAbsAppPath('/test'):", abp.toAbsAppPath('/test'));
    console.log("abp.toAbsAppPath('test'):", abp.toAbsAppPath('test'));

    // 2. Logging
    console.log("Testing Logging (check console output):");
    abp.log.debug("This is a debug message");
    abp.log.info("This is an info message");
    abp.log.warn("This is a warning message");
    abp.log.error("This is an error message");
    abp.log.fatal("This is a fatal message");

    // 3. Localization
    console.log("Testing Localization:");
    abp.localization.values['MySource'] = { 'Hello': 'Hola {0}!' };
    console.log("abp.localization.localize('Hello', 'MySource', 'Mundo'):", abp.localization.localize('Hello', 'MySource', 'Mundo'));
    console.log("abp.localization.isLocalized('Hello', 'MySource'):", abp.localization.isLocalized('Hello', 'MySource'));
    console.log("abp.localization.isLocalized('NonExistent', 'MySource'):", abp.localization.isLocalized('NonExistent', 'MySource'));

    // 4. Authorization
    console.log("Testing Authorization:");
    abp.auth.grantedPolicies['MyPolicy'] = true;
    console.log("abp.auth.isGranted('MyPolicy'):", abp.auth.isGranted('MyPolicy'));
    console.log("abp.auth.isGranted('OtherPolicy'):", abp.auth.isGranted('OtherPolicy'));
    console.log("abp.auth.isAnyGranted('MyPolicy', 'OtherPolicy'):", abp.auth.isAnyGranted('MyPolicy', 'OtherPolicy'));
    console.log("abp.auth.areAllGranted('MyPolicy', 'OtherPolicy'):", abp.auth.areAllGranted('MyPolicy', 'OtherPolicy'));

    // 5. Settings
    console.log("Testing Settings:");
    abp.setting.values['MySetting'] = 'true';
    abp.setting.values['MyIntSetting'] = '123';
    console.log("abp.setting.get('MySetting'):", abp.setting.get('MySetting'));
    console.log("abp.setting.getBoolean('MySetting'):", abp.setting.getBoolean('MySetting'));
    console.log("abp.setting.getInt('MyIntSetting'):", abp.setting.getInt('MyIntSetting'));

    // 6. Notification & Message (Stubs)
    console.log("Testing Notification & Message (mostly stubs):");
    abp.notify.success("Success notification");
    // abp.message.info("Info message"); // This triggers alert()

    // 7. UI (Block/Unblock)
    console.log("Testing UI Block/Unblock (check for overlay):");
    abp.ui.setBusy();
    setTimeout(function() {
        abp.ui.clearBusy();
        console.log("UI Unblocked after 1 second");
    }, 1000);

    // 8. Event Bus
    console.log("Testing Event Bus:");
    var eventHandler = function(data) {
        console.log("Event 'testEvent' triggered with data:", data);
    };
    abp.event.on('testEvent', eventHandler);
    abp.event.trigger('testEvent', { foo: 'bar' });
    abp.event.off('testEvent', eventHandler);
    abp.event.trigger('testEvent', { foo: 'bar' }); // Should not log anything

    // 9. Utils
    console.log("Testing Utils:");
    console.log("abp.utils.toPascalCase('helloWorld'):", abp.utils.toPascalCase('helloWorld'));
    console.log("abp.utils.toCamelCase('HelloWorld'):", abp.utils.toCamelCase('HelloWorld'));
    console.log("abp.utils.truncateString('LongString', 4):", abp.utils.truncateString('LongString', 4));
    console.log("abp.utils.truncateStringWithPostfix('LongString', 7, '...'):", abp.utils.truncateStringWithPostfix('LongString', 7, '...'));
    console.log("abp.utils.formatString('Hello {0}', 'World'):", abp.utils.formatString('Hello {0}', 'World'));
    console.log("abp.utils.htmlEscape('<div>Test</div>'):", abp.utils.htmlEscape('<div>Test</div>'));

    // 10. Clock
    console.log("Testing Clock:");
    console.log("abp.clock.now():", abp.clock.now());
    console.log("abp.clock.kind:", abp.clock.kind);

    // 11. Features
    console.log("Testing Features:");
    abp.features.values['MyFeature'] = 'true';
    console.log("abp.features.isEnabled('MyFeature'):", abp.features.isEnabled('MyFeature'));

    console.log("--- abp.js tests completed ---");
})();

(function(){
    
})();
