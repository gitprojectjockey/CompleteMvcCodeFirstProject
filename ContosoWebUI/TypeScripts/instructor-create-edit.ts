/// <reference path="../scripts/typings/jquery/jquery.d.ts" />

declare var $: JQueryStatic;
namespace Instructor.CreateAndEdit {

    export function init() {
        //Types
        interface ICourse {
            code: string;
            name: string;
            courseInfo: () => string;
        }

        class Course implements ICourse {
            constructor(code: string, name: string) {
                this.code = code;
                this.name = name;
            }

            code: string;
            name: string;
            courseInfo() {
                return this.code + " " + this.name;
            }
        }

        //Events
        $(".course tr td input").change(function (e) {
            var codeList: Array<Course> = GetSelectedCourses()
            $('#selectedCourseList').empty();
            codeList.forEach(function (c: Course) {
                $('#selectedCourseList').append('<h4>' + c.courseInfo() + '</h4>')
            });
        });

        $('#btnInstructor-Create-Save').click(function (e) {
            let atleastOneIsChecked = ValidateCoursesChecked();
            return atleastOneIsChecked;
        });

        //functions
        function GetSelectedCourses(): Array<Course> {
            var list: Array<Course> = [];
            $('.course input:checkbox:checked').each(function () {
                let code = $(this).parent().siblings().eq(0).text().trim();
                let name = $(this).parent().siblings().eq(1).text().trim();
                var course = new Course(code, name);
                course.code = code;
                list.push(course);
            });
            return list;
        }

        function ValidateCoursesChecked(): boolean {
            let checked: boolean = false;
            $('.course input:checkbox:checked').each(function () {
                checked = true
            });
            return checked;
        }

        function InitCourses() {
            let codeList: Array<Course> = GetSelectedCourses()
            $('#selectedCourseList').empty();
            codeList.forEach(function (c: Course) {
                $('#selectedCourseList').append('<h4>' + c.courseInfo() + '</h4>')
            });
        }

        InitCourses();
    }
}

$(document).ready(function () {
    //only init instructor-create-edit.js if it is the instructor.edit or instrunctor.create pages
    if ($('#0929AA6C-6367-414E-87FD-F89151019A7B').length > 0 || $('#AB7BEE7D-141F-4C23-915A-A4EDD8346790').length > 0) {
        Instructor.CreateAndEdit.init();
    }
});