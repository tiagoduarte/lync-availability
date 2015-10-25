
LyncAvailability Console Application

- Displays the status of a Lync contact, based on its email address
- Is there is idle time information, display it as well
- Handle exceptions and pre-requisites

*** FAQ ***

Problem: application says that there are no results

Solution: double check the input email address. It is possible that there are different 
values for 'Email Address' and 'IM Address' fields

***

Problem: Getting erroneous/innacurate values

Solution: cleanup the outlook cache and the lync cache

Cleaning up outlook cache:
Go to Outlook Options > Email > Click "Empty Auto-Complete List".
Also, you may want/need to delete all contacts from the Suggested Contacts group

Clearing up the lync cache:
Delete your 'sip' folder from
%UserProfile%\AppData\Local\Microsoft\Office\15.0\Lync

***

Problem: Application hangs

Solution: Stop trying to get the elapsed time (GetElapsedIdleTime)
