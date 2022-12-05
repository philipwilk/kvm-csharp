## Pre-run instructions
Requires an increased memory lock limit, which **should** be done by doing ```ulimit -l unlimited```. However, as most modern distros use systemd, it is *almost* certain ***you will be required to*** set the value:
 ```DefaultLimitMEMLOCK=infinity``` in ``/etc/systemd/system.conf`` and ``/etc/systemd/user.conf``. This is explained in [this stackoverflow post - handling system limits with systemd.](https://github.com/clearlinux/distribution/issues/2372#issuecomment-861757594)
## Instructions to run
Needs you to do  ``export LD_LIBRARY_PATH=~/Documents/kvm-csharp/ioctls/build/`` to run, where ``~/Documents/kvm-csharp`` is the repo path, or, if you're lazy like me, symlink the directory of the builds in ``ioctl/builds`` to ``/usr/lib64/libKVM_IOCTLS.so`` and forget about it.
Once that's done, you can run the program just by doing ```dotnet run``` in the repo directory. 