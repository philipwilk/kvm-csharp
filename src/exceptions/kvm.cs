
namespace Main
{
  public class vm_fd_not_created : SystemException
  {
    public vm_fd_not_created(int fd) : base(String.Format("Error while creating vm file descriptor: received file descriptor as {0} from kernel.", fd)) { }
  }

  public class user_memory_region_not_set : SystemException
  {
    public user_memory_region_not_set(ulong capacity, int vm_fd) : base(String.Format("Failed to set user memory region of {0} bytes on vm {1}.", capacity, vm_fd)) { }
  }

  public class not_enough_logical_processors : ArgumentOutOfRangeException
  {
    public not_enough_logical_processors(uint vcpu, int logicals) : base(String.Format("Requested vm with {0} vcpu, but only {1} logical processors available on host.", vcpu, logicals)) { }
  }

  public class failed_creating_vcpu : SystemException
  {
    public failed_creating_vcpu(uint vcpu, int vm_fd) : base(String.Format("Failed to create vcpu {0} for vm with fd {1}.", vcpu, vm_fd)) { }
  }

  public class failed_setting_sregs : SystemException
  {
    public failed_setting_sregs(uint vcpu, int vm_fd) : base(String.Format("Failed to set special registers for vcpu {0} for vm with fd {1}.", vcpu.ToString(), vm_fd.ToString())) { }
  }
  public class failed_setting_regs : SystemException
  {
    public failed_setting_regs(uint vcpu, int vm_fd) : base(String.Format("Failed to set registers for vcpu {0} for vm with fd {1}.", vcpu.ToString(), vm_fd.ToString())) { }
  }

  public class failed_setting_tss_addr : SystemException
  {
    public failed_setting_tss_addr(int vm_fd) : base(String.Format("Failed to set tss address for vm with fd {1}.", vm_fd.ToString())) { }
  }

  public class failed_setting_identity_map_addr : SystemException
  {
    public failed_setting_identity_map_addr(int vm_fd) : base(String.Format("Failed to set identity map address for vm with fd {1}.", vm_fd.ToString())) { }
  }

  public class failed_creating_irqchip : SystemException
  {
    public failed_creating_irqchip(int vm_fd) : base(String.Format("Failed to create interupt request chip (irq) for vm with fd {1}.", vm_fd.ToString())) { }
  }

  public class failed_creating_pit2 : SystemException
  {
    public failed_creating_pit2(int vm_fd) : base(String.Format("Failed to create in-kernel device model for i8254 PIT for vm with fd {1}.", vm_fd.ToString())) { }
  }

  public abstract class less_than_one : ArgumentOutOfRangeException
  {
    public less_than_one(string parameter) : base(String.Format("Cannot create a vm with less than 1 {1}.", parameter)) { }
  }
  public class less_than_one_vcpu : less_than_one
  {
    public less_than_one_vcpu() : base("vcpu") { }
  }

  public class less_than_one_memory : less_than_one
  {
    public less_than_one_memory() : base("mebibyte of memory") { }
  }
}
