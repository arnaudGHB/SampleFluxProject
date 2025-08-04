using AutoMapper;
using CBS.Customer.MEDIATR;
using CBS.CUSTOMER.DATA.Dto;
using System.Collections.Generic;

namespace CBS.CUSTOMER.API.Helpers.MappingProfile
{
    public class EmployeeProfile : Profile
    {
        public EmployeeProfile()
        {
            CreateMap<DATA.Entity.Employee, CreateEmployee>().ReverseMap();
             
            CreateMap <AddEmployeeCommand, DATA.Entity.Employee >().ReverseMap();


            CreateMap<DATA.Entity.EmployeeLeave, CreateEmployeeLeave>().ReverseMap();
            CreateMap<AddEmployeeLeaveCommand, DATA.Entity.EmployeeLeave>().ReverseMap();     
            
            CreateMap<DATA.Entity.EmployeeTraining, CreateEmployeeTraining>().ReverseMap();
            CreateMap<AddEmployeeTrainingCommand, DATA.Entity.EmployeeTraining>().ReverseMap();    
            
            
            CreateMap<DATA.Entity.LeaveType, CreateLeaveType>().ReverseMap();
            CreateMap<AddLeaveTypeCommand, DATA.Entity.LeaveType>().ReverseMap();


            CreateMap<DATA.Entity.JobTitle, CreateJobTitle>().ReverseMap();
            CreateMap<AddJobTitleCommand, DATA.Entity.JobTitle>().ReverseMap();          
            
            
            CreateMap<DATA.Entity.Department, CreateDepartment>().ReverseMap();
            CreateMap<AddDepartmentCommand, DATA.Entity.Department>().ReverseMap();       
            
            
            CreateMap<DATA.Entity.Employee, UpdateEmployee>().ReverseMap();
            CreateMap<UpdateEmployeeCommand, DATA.Entity.Employee>().ReverseMap();  
            
            
            CreateMap<DATA.Entity.EmployeeLeave, UpdateEmployeeLeave>().ReverseMap();
            CreateMap<UpdateEmployeeLeaveCommand, DATA.Entity.EmployeeLeave>().ReverseMap();

            CreateMap<DATA.Entity.Employee,GetAllEmployees>().ReverseMap();
            CreateMap<DATA.Entity.EmployeeLeave ,GetEmployeeLeave>().ReverseMap();
            CreateMap<  DATA.Entity.LeaveType, GetLeaveType>().ReverseMap();
            CreateMap<DATA.Entity.JobTitle, GetJobTitle> ().ReverseMap();
            CreateMap <  DATA.Entity.Department  ,GetDepartment>().ReverseMap();
            CreateMap< DATA.Entity.Employee, GetEmployee>().ReverseMap();
            CreateMap<DATA.Entity.EmployeeLeave,GetEmployeeLeave >().ReverseMap();

        }
    }
}
