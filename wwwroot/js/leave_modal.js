
const leaveUrls = {
    getLeaveById: './Leave/GetLeaveByID',
    getLeaves: './Leave/GetLeaves',
    createRequest: './Leave/CreateRequest',
    uploadTempFile: './Leave/UploadTempFile',
    deleteTemp: './Leave/DeleteTemp',
    getTempFileInfo: './Leave/GetTempFileInfo'
};

function initLeaveModal(empId, workingDate, holidays = []) {
    let leaves = [];
    let balance_leave = 0;
    let type_duration = "Day";
    let file_require = false;
    let is_full_day = true;
    let gender = "";
    let tempFileIds = [];
    const hourSelect = document.getElementById('leave_start_hour');
    const leaveFullBtn = document.getElementById('leave_full');
    const leaveHalfBtn = document.getElementById('leave_half');
    const leaveHoursBtn = document.getElementById('leave_hours');
    const dateGroup = document.getElementById('leave_date_group');
    const attachFileGroup = document.getElementById('leave_attach_file_group');
    const timeGroup = document.getElementById('leave_time_group');
    const durationSelect = timeGroup.querySelector('select');
    const startHourTimeInput = document.getElementById('leave_start_hour');
    const startMinuteTimeInput = document.getElementById('leave_start_minute');
    const startDateInput = document.getElementById('leave_start_date');
    const stopTimeInput = document.getElementById('leave_stop_time');
    const stopDateInput = document.getElementById('leave_stop_date');
    const daysInput = dateGroup.querySelector('input[type="number"]');
    const fileInput = document.getElementById('leave_fileInput');
    const selectFileBtn = document.getElementById('selectFileBtn');
    const uploadForm = document.getElementById('uploadForm');
    const fileListContainer = document.getElementById('leave_fileListContainer');

    for (let h = 8; h <= 15; h++) {
        const option = document.createElement('option');
        option.value = String(h).padStart(2, '0');
        option.textContent = String(h).padStart(2, '0');
        hourSelect.appendChild(option);
    }
    hourSelect.value = '08';

    $('#leave').on('click', async function () {
        $('#modal_leave').modal();
        $('#modal_task').modal('hide');
        $('#leave_start_date').val(workingDate);
        $('#leave_stop_date').val(workingDate);
        $('#div_leave').prop('hidden', true);
        $('#div_main_leave').css('height', '100px');
        $('#div_main_leave').css('overflow-y', 'hidden');
        $('#btn_leave_send').prop('disabled', true);
        await GetLeaveType(empId);
    });

    $('#select_leave_type').on('change', async function () {
        let leave_type_id = $('#select_leave_type').val();
        let request_date = workingDate;
        let leave_ = leaves.filter(f => f.leave_type_id === leave_type_id)[0];
        let description = leave_.description;
        let year = new Date(request_date).getFullYear();
        let hire_date = new Date();
        let leave;
        $('#leave_stop_date').val(request_date);
        $('#amount_leave').val(1);
        await $.ajax({
            type: "GET",
            url: leaveUrls.getLeaveById,
            contentType: 'application/x-www-form-urlencoded',
            data: { leave_type_id, emp_id: empId, year },
            success: function (response) {
                leave = response.leave;
                balance_leave = response.balance;
                gender = response.gender;
                hire_date = response.hire_date;
            }
        });
        let chk_request_timing = CheckRequestTiming(request_date);
        let amount_diff_day = diffDays(new Date(), hire_date);
        if (amount_diff_day > leave.length_start_date) {
            if (chk_request_timing) {
                let chk_gender_restriction = CheckGenderRestriction(gender);
                if (chk_gender_restriction) {
                    $('#leave_description').text(description);
                    $('#leave_balance').text(balance_leave + '/' + leave.amount_entitlement + ' วัน');
                    $('#div_leave').prop('hidden', false);
                    $('#div_main_leave').css('height', '500px');
                    $('#div_main_leave').css('overflow-y', 'auto');
                    $('#btn_leave_send').prop('disabled', false);
                    resetButtonStyles();
                    leaveFullBtn.classList.add('active');
                    toggleGroupDisplay(dateGroup, true);
                    toggleGroupDisplay(timeGroup, false);
                    daysInput.value = 1;
                    stopDateInput.value = startDateInput.value;
                    stopDateInput.disabled = false;
                    type_duration = "Day";
                    CheckAttachFileRequire(type_duration);
                    $('#leave_full').prop('disabled', false);
                    $('#leave_half').prop('disabled', false);
                    $('#leave_hours').prop('disabled', false);
                    $('#leave_stop_date').prop('disabled', false);
                } else {
                    alert('ไม่สามารถลาได้เนื่องจากเพศสภาพไม่ตรงกับการลา');
                    $('#btn_leave_send').prop('disabled', true);
                    $('#leave_full').prop('disabled', true);
                    $('#leave_half').prop('disabled', true);
                    $('#leave_hours').prop('disabled', true);
                    $('#leave_stop_date').prop('disabled', true);
                }
            } else {
                alert('ไม่สามารถลาได้เนื่องจากช่วงเวลาการลาไม่ถูกต้อง');
                $('#btn_leave_send').prop('disabled', true);
                $('#leave_full').prop('disabled', true);
                $('#leave_half').prop('disabled', true);
                $('#leave_hours').prop('disabled', true);
                $('#leave_stop_date').prop('disabled', true);
            }
        } else {
            alert('ไม่สามารถลาได้เนื่องจากวันทำงานยังไม่ครบกำหนด');
            $('#btn_leave_send').prop('disabled', true);
            $('#leave_full').prop('disabled', true);
            $('#leave_half').prop('disabled', true);
            $('#leave_hours').prop('disabled', true);
            $('#leave_stop_date').prop('disabled', true);
        }
        is_full_day = true;
    });

    function diffDays(date1, date2) {
        const d1 = new Date(date1);
        const d2 = new Date(date2);
        d1.setHours(0, 0, 0, 0);
        d2.setHours(0, 0, 0, 0);
        const diffTime = Math.abs(d2 - d1);
        const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
        return diffDays;
    }

    function resetButtonStyles() {
        leaveFullBtn.classList.remove('active');
        leaveHalfBtn.classList.remove('active');
        leaveHoursBtn.classList.remove('active');
    }

    function setHalfDayOptions() {
        durationSelect.innerHTML = `
            <option value="" selected disabled>Select</option>
            <option value="morning">ครึ่งวันเช้า</option>
            <option value="afternoon">ครึ่งวันบ่าย</option>
        `;
        startHourTimeInput.value = '08';
        startMinuteTimeInput.value = '00';
        stopTimeInput.value = '';
        startHourTimeInput.disabled = false;
        startMinuteTimeInput.disabled = false;
        daysInput.value = 0;
        stopDateInput.value = startDateInput.value;
        stopDateInput.disabled = true;
        durationSelect.onchange = function () {
            if (this.value === 'morning') {
                startHourTimeInput.value = '08';
                startMinuteTimeInput.value = '30';
                stopTimeInput.value = '12:00';
                startHourTimeInput.disabled = true;
                startMinuteTimeInput.disabled = true;
            } else if (this.value === 'afternoon') {
                startHourTimeInput.value = '13';
                startMinuteTimeInput.value = '00';
                stopTimeInput.value = '17:30';
                startHourTimeInput.disabled = true;
                startMinuteTimeInput.disabled = true;
            } else {
                startHourTimeInput.value = '08';
                startMinuteTimeInput.value = '30';
                stopTimeInput.value = '';
                startHourTimeInput.disabled = false;
                startMinuteTimeInput.disabled = false;
            }
            if (balance_leave < 0.5) {
                alert('วันลาไม่เพียงพอ');
                $('#btn_leave_send').prop('disabled', true);
            } else {
                $('#btn_leave_send').prop('disabled', false);
            }
        };
    }

    function setHourlyOptions() {
        durationSelect.innerHTML = `
            <option value="" selected disabled>Select</option>
            <option value="2">2 ชั่วโมง</option>
            <option value="4">4 ชั่วโมง</option>
            <option value="6">6 ชั่วโมง</option>
        `;
        startHourTimeInput.value = '08';
        startMinuteTimeInput.value = '30';
        stopTimeInput.value = '';
        startHourTimeInput.disabled = false;
        startMinuteTimeInput.disabled = false;
        daysInput.value = 0;
        stopDateInput.value = startDateInput.value;
        stopDateInput.disabled = true;
        durationSelect.onchange = function () {
            calculateTime();
            startHourTimeInput.disabled = false;
            startMinuteTimeInput.disabled = false;
            let duration = parseInt(durationSelect.value);
            let hour = (duration / 8);
            if (balance_leave < hour) {
                alert('วันลาไม่เพียงพอ');
                $('#btn_leave_send').prop('disabled', true);
            } else {
                $('#btn_leave_send').prop('disabled', false);
            }
        };
        startHourTimeInput.onchange = function () {
            calculateTime();
        };
        startMinuteTimeInput.onchange = function () {
            calculateTime();
        };
    }

    function toggleGroupDisplay(group, show) {
        if (show) {
            group.style.display = 'flex';
            group.style.opacity = '0';
            group.style.transition = 'opacity 0.3s ease-in-out';
            setTimeout(() => {
                group.style.opacity = '1';
            }, 10);
        } else {
            group.style.opacity = '0';
            group.style.transition = 'opacity 0.3s ease-in-out';
            setTimeout(() => {
                group.style.display = 'none';
                if (group === timeGroup) {
                    startHourTimeInput.value = '';
                    startMinuteTimeInput.value = '';
                    stopTimeInput.value = '';
                    durationSelect.value = '';
                    startHourTimeInput.disabled = false;
                    startMinuteTimeInput.disabled = false;
                }
            }, 300);
        }
    }

    leaveFullBtn.addEventListener('click', function () {
        resetButtonStyles();
        leaveFullBtn.classList.add('active');
        toggleGroupDisplay(dateGroup, true);
        toggleGroupDisplay(timeGroup, false);
        daysInput.value = 1;
        stopDateInput.value = startDateInput.value;
        stopDateInput.disabled = false;
        type_duration = "Day";
        CheckAttachFileRequire(type_duration);
        is_full_day = true;
    });

    leaveHalfBtn.addEventListener('click', function () {
        resetButtonStyles();
        leaveHalfBtn.classList.add('active');
        toggleGroupDisplay(dateGroup, true);
        toggleGroupDisplay(timeGroup, true);
        setHalfDayOptions();
        type_duration = "Half";
        CheckAttachFileRequire(type_duration);
        is_full_day = false;
    });

    leaveHoursBtn.addEventListener('click', function () {
        resetButtonStyles();
        leaveHoursBtn.classList.add('active');
        toggleGroupDisplay(dateGroup, true);
        toggleGroupDisplay(timeGroup, true);
        setHourlyOptions();
        type_duration = "Hours";
        CheckAttachFileRequire(type_duration);
        is_full_day = false;
    });

    stopDateInput.addEventListener('change', function () {
        type_duration = "Day";
        CheckAttachFileRequire(type_duration);
    });

    function CheckAttachFileRequire(type_duration) {
        $('#btn_leave_send').prop('disabled', true);
        let leave_type_id = $('#select_leave_type').val();
        let _leave = leaves.filter(f => f.leave_type_id === leave_type_id)[0];
        if (type_duration == "Day") {
            if (startDateInput.value !== "" && stopDateInput.value !== "") {
                let start = new Date(startDateInput.value);
                let stop = new Date(stopDateInput.value);
                let diffInDays = 0;
                if (stop >= start) {
                    if (_leave.count_holidays_as_leave === true) {
                        diffInDays = getDaysBetween(start, stop, true);
                    } else {
                        diffInDays = getBusinessDays(start, stop, holidays);
                    }
                    if (balance_leave >= diffInDays) {
                        if (_leave.is_consecutive === true) {
                            if (_leave.max_consecutive_days >= diffInDays) {
                                daysInput.value = diffInDays;
                            } else {
                                alert(`ไม่สามารถลาเกิน ${_leave.max_consecutive_days} วัน`);
                                daysInput.value = 1;
                                stopDateInput.value = workingDate;
                            }
                        } else {
                            daysInput.value = diffInDays;
                        }
                        $('#btn_leave_send').prop('disabled', false);
                    } else {
                        alert('วันลาไม่เพียงพอ');
                        daysInput.value = 1;
                        stopDateInput.value = workingDate;
                        $('#btn_leave_send').prop('disabled', true);
                    }
                } else {
                    alert('ใส่วันผิด');
                    daysInput.value = 1;
                    stopDateInput.value = workingDate;
                }
                if (_leave.attachment_required === true) {
                    if (diffInDays >= Number(_leave.attachment_threshold_days)) {
                        attachFileGroup.hidden = false;
                        file_require = true;
                    } else {
                        clearAttachFile();
                    }
                } else {
                    clearAttachFile();
                }
            }
        } else if (type_duration === "Half") {
            if (_leave.attachment_required === true) {
                if (Number(_leave.attachment_threshold_days) === 0) {
                    attachFileGroup.hidden = false;
                    file_require = true;
                } else {
                    clearAttachFile();
                }
            } else {
                clearAttachFile();
            }
        } else if (type_duration === "Hours") {
            if (_leave.attachment_required === true) {
                if (Number(_leave.attachment_threshold_days) === 0) {
                    attachFileGroup.hidden = false;
                    file_require = true;
                } else {
                    clearAttachFile();
                }
            } else {
                clearAttachFile();
            }
        }
    }

    function CheckRequestTiming(request_date) {
        let leave_type_id = $('#select_leave_type').val();
        let leave = leaves.filter(f => f.leave_type_id === leave_type_id)[0];
        let date_now = new Date();
        let date_request = new Date(request_date);
        date_now.setHours(0, 0, 0, 0);
        date_request.setHours(0, 0, 0, 0);
        if (leave.request_timing === "Both") {
            return true;
        } else if (leave.request_timing === "Future") {
            if (date_request > date_now) {
                return true;
            } else {
                return false;
            }
        } else if (leave.request_timing === "Past") {
            if (date_request <= date_now) {
                return true;
            } else {
                return false;
            }
        }
    }

    function CheckGenderRestriction(gender) {
        let leave_type_id = $('#select_leave_type').val();
        let leave = leaves.filter(f => f.leave_type_id === leave_type_id)[0];
        if (leave.gender_restriction === "Both") {
            return true;
        } else {
            if (leave.gender_restriction === gender) {
                return true;
            } else {
                return false;
            }
        }
    }

    function clearAttachFile() {
        ClearAttachFile();
        attachFileGroup.hidden = true;
        loadExistingFiles([]);
        file_require = false;
    }

    function getDaysBetween(startDate, endDate, absolute = true) {
        const start = new Date(startDate);
        const end = new Date(endDate);
        const diffInMs = end - start;
        const diffInDays = diffInMs / (1000 * 60 * 60 * 24);
        const days = Math.floor(diffInDays) + 1;
        return absolute ? Math.abs(days) : days;
    }

    function getBusinessDays(startDate, endDate, holidays = []) {
        const start = new Date(startDate);
        const end = new Date(endDate);
        let count = 0;
        const current = new Date(start);
        const holidaySet = new Set(
            holidays.map(date => {
                const d = new Date(date);
                return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`;
            })
        );
        while (current <= end) {
            const dayOfWeek = current.getDay();
            const dateStr = current.toISOString().split('T')[0];
            if (dayOfWeek !== 0 && dayOfWeek !== 6 && !holidaySet.has(dateStr)) {
                count++;
            }
            current.setDate(current.getDate() + 1);
        }
        return count;
    }

    function timeToMinutes(timeStr) {
        const [hours, minutes] = timeStr.split(':').map(Number);
        return hours * 60 + minutes;
    }

    function calculateTime() {
        let first = timeToMinutes("08:30");
        let last = timeToMinutes("17:30");
        let duration = parseInt(durationSelect.value);
        let start = startHourTimeInput.value + ':' + startMinuteTimeInput.value;
        if (timeToMinutes(start) >= first && timeToMinutes(start) <= last) {
            let [hours, minutes] = start.split(':').map(Number);
            let date = new Date(startDateInput.value);
            date.setHours(hours);
            date.setMinutes(minutes);
            date.setSeconds(0);
            date.setHours(date.getHours() + duration);
            if (timeToMinutes(date.toTimeString().split(' ')[0]) <= last) {
                if (parseInt(startDateInput.value) <= 12 || (date.getHours() === 12 && date.getMinutes() > 0) || (date.getHours() > 12)) {
                    date.setHours(date.getHours() + 1);
                }
                if (timeToMinutes(date.toTimeString().split(' ')[0]) <= last) {
                    let stopHours = String(date.getHours()).padStart(2, '0');
                    let stopMinutes = String(date.getMinutes()).padStart(2, '0');
                    let stopTime = `${stopHours}:${stopMinutes}`;
                    stopTimeInput.value = stopTime;
                } else {
                    alert('ใส่เวลาผิด');
                    startHourTimeInput.value = '08';
                    startMinuteTimeInput.value = '30';
                    date.setHours(8);
                    date.setMinutes(30);
                    date.setHours(date.getHours() + duration);
                    let stopHours = String(date.getHours()).padStart(2, '0');
                    let stopMinutes = String(date.getMinutes()).padStart(2, '0');
                    let stopTime = `${stopHours}:${stopMinutes}`;
                    stopTimeInput.value = stopTime;
                }
            } else {
                alert('ใส่เวลาผิด');
                startHourTimeInput.value = '08';
                startMinuteTimeInput.value = '30';
                date.setHours(8);
                date.setMinutes(30);
                date.setHours(date.getHours() + duration);
                let stopHours = String(date.getHours()).padStart(2, '0');
                let stopMinutes = String(date.getMinutes()).padStart(2, '0');
                let stopTime = `${stopHours}:${stopMinutes}`;
                stopTimeInput.value = stopTime;
            }
        } else {
            alert('ใส่เวลาผิด');
            startHourTimeInput.value = '08';
            startMinuteTimeInput.value = '30';
            stopTimeInput.value = "";
        }
    }

    leaveFullBtn.click();

    if (!fileInput || !selectFileBtn || !uploadForm || !fileListContainer) {
        console.error('ไม่พบ element');
    }

    selectFileBtn.addEventListener('click', (e) => {
        e.preventDefault();
        fileInput.click();
    });

    ['dragenter', 'dragover'].forEach(event => {
        uploadForm.addEventListener(event, (e) => {
            e.preventDefault();
            e.stopPropagation();
            uploadForm.classList.add('dragover');
        });
    });

    ['dragleave', 'drop'].forEach(event => {
        uploadForm.addEventListener(event, (e) => {
            e.preventDefault();
            e.stopPropagation();
            uploadForm.classList.remove('dragover');
            if (event === 'drop') handleFiles(e.dataTransfer.files);
        });
    });

    fileInput.addEventListener('change', () => {
        handleFiles(fileInput.files);
        fileInput.value = '';
    });

    async function handleFiles(files) {
        if (!files || files.length === 0) return;
        for (let file of files) {
            const fileType = file.name.split('.').pop().toLowerCase();
            const allowedTypes = ['pdf', 'jpg', 'jpeg', 'png', 'xls', 'xlsx'];
            const maxSize = 10 * 1024 * 1024; // 10MB
            if (!allowedTypes.includes(fileType)) {
                showAlert(`ประเภทไฟล์ไม่รองรับ: ${file.name}`);
                continue;
            }
            if (file.size > maxSize) {
                showAlert(`ไฟล์ใหญ่เกิน 10MB: ${file.name}`);
                continue;
            }
            const tempId = await uploadFileToTemp(file);
            if (tempId) {
                tempFileIds.push(tempId.tempId);
                const fileItem = createFileItem(file.name, fileType, tempId.tempId, tempId.previewUrl);
                fileListContainer.appendChild(fileItem);
                setTimeout(() => fileItem.classList.add('show'), 10);
            }
        }
    }

    async function uploadFileToTemp(file) {
        const formData = new FormData();
        formData.append('file', file);
        try {
            const response = await fetch(leaveUrls.uploadTempFile, {
                method: 'POST',
                body: formData
            });
            if (response.ok) {
                const result = await response.json();
                return result;
            } else {
                const error = await response.text();
                showAlert(`อัปโหลดล้มเหลว: ${error}`);
                return null;
            }
        } catch (err) {
            showAlert(`เกิดข้อผิดพลาด: ${err.message}`);
            return null;
        }
    }

    function createFileItem(fileName, fileType, tempId, fileUrl) {
        const fileItem = document.createElement('div');
        fileItem.className = 'list-group-item d-flex align-items-center file-item added';
        fileItem.dataset.tempId = tempId;
        const viewLink = document.createElement('a');
        viewLink.href = fileUrl;
        viewLink.target = '_blank';
        viewLink.rel = 'noopener';
        viewLink.className = 'text-decoration-none flex-grow-1 d-flex align-items-center';
        viewLink.style.cursor = 'pointer';
        const iconClass = fileType === 'pdf'
            ? 'fas fa-file-pdf text-danger'
            : 'fas fa-file-image text-primary';
        viewLink.innerHTML = `
            <i class="${iconClass} mr-2"></i>
            <span class="kanit-regular text-dark">${fileName}</span>
        `;
        const deleteBtn = document.createElement('button');
        deleteBtn.type = 'button';
        deleteBtn.className = 'btn btn-outline-danger btn-sm delete-btn ml-2';
        deleteBtn.dataset.tempId = tempId;
        deleteBtn.title = 'ลบ';
        deleteBtn.innerHTML = '<i class="fas fa-trash-alt"></i>';
        fileItem.appendChild(viewLink);
        fileItem.appendChild(deleteBtn);
        return fileItem;
    }

    fileListContainer.addEventListener('click', async (e) => {
        const btn = e.target.closest('.delete-btn');
        if (!btn) return;
        const fileItem = btn.closest('.file-item');
        const tempId = btn.dataset.tempId;
        try {
            await fetch(`${leaveUrls.deleteTemp}?tempId=${tempId}`, { method: 'DELETE' });
        } catch (err) {
            console.error('ลบไฟล์ล้มเหลว', err);
        }
        tempFileIds = tempFileIds.filter(id => id !== tempId);
        fileItem.classList.add('removing');
        setTimeout(() => fileItem.remove(), 300);
    });

    function loadExistingFiles(existingTempIds) {
        fileListContainer.innerHTML = '';
        tempFileIds = [];
        if (!Array.isArray(existingTempIds)) return;
        existingTempIds.forEach(async (tempId) => {
            const response = await fetch(`${leaveUrls.getTempFileInfo}?tempId=${tempId}`);
            if (response.ok) {
                const info = await response.json();
                tempFileIds.push(tempId);
                const fileItem = createFileItem(info.fileName, info.fileType, tempId, "");
                fileListContainer.appendChild(fileItem);
                fileItem.classList.add('show');
            }
        });
    }

    function ModalCloseClick() {
        ClearAttachFile();
    }

    async function ClearAttachFile() {
        for (let i = 0; i < tempFileIds.length; i++) {
            try {
                await fetch(`${leaveUrls.deleteTemp}?tempId=${tempFileIds[i]}`, { method: 'DELETE' });
            } catch (err) {
                console.error('ลบไฟล์ล้มเหลว', err);
            }
        }
        fileListContainer.innerHTML = '';
        tempFileIds = [];
    }

    function showAlert(message) {
        const modalAlert = document.getElementById('modal_alert');
        const alertContent = document.getElementById('alert_content');
        if (modalAlert && alertContent) {
            alertContent.innerHTML = `<p class="kanit-regular">${message}</p>`;
            $(modalAlert).modal('show');
        } else {
            alert(message);
        }
    }

    async function GetLeaveType(empId) {
        let now = new Date();
        let year = now.getFullYear();
        await $.ajax({
            type: "GET",
            url: leaveUrls.getLeaves,
            contentType: 'application/x-www-form-urlencoded',
            data: { emp_id: empId, year },
            success: function (response) {
                leaves = response.leaves;
                GenerateLeaveOption(leaves);
            }
        });
    }

    function GenerateLeaveOption(leaves) {
        $('#select_leave_type').empty();
        $('#select_leave_type').append(`<option value="" selected disabled>Select</option>`);
        for (let i = 0; i < leaves.length; i++) {
            if (leaves[i].is_active) {
                $('#select_leave_type').append(`<option value="${leaves[i].leave_type_id}">${leaves[i].leave_name_th}</option>`);
            }
        }
    }

    $('#btn_leave_send').on('click', async function () {
        let leave_type = $('#select_leave_type :selected').text();
        let date_start = formatDate(new Date($('#leave_start_date').val()));
        let date_stop = formatDate(new Date($('#leave_stop_date').val()));
        let hour_start = $('#leave_start_hour').val();
        let minute_start = $('#leave_start_minute').val();
        let time_stop = $('#leave_stop_time').val();
        let message = "";
        let year_start = new Date($('#leave_start_date').val()).getFullYear();
        let year_stop = new Date($('#leave_stop_date').val()).getFullYear();
        if (year_start === year_stop) {
            if (is_full_day) {
                message = `${leave_type}
                    ลาตั้งแต่วันที่ ${date_start} - ${date_stop}`;
            } else {
                message = `${leave_type}
                    ลาวันที่ ${date_start}
                    เวลา ${hour_start}:${minute_start} - ${time_stop}`;
            }
            $('#confirm_description').text(message);
            $('#modal_confirm').modal();
        } else {
            alert('ไม่สามารถลาข้ามปีได้');
        }
    });

    $('#confirm_ok').on('click', async function () {
        $('#confirm_ok').attr('disabled', true);
        $('#SpinModal').modal();
        let leave_type_id = $('#select_leave_type').val();
        let leave = leaves.filter(f => f.leave_type_id === leave_type_id)[0];
        let leave_type_code = leave.leave_type_code;
        let is_two_step_approve = leave.is_two_step_approve;
        let start_request_date = $('#leave_start_date').val();
        let end_request_date = $('#leave_stop_date').val();
        let description = $('#leave_note').val();
        let amount_leave_day = $('#amount_leave').val();
        let start_request_time = "00:00:00";
        let end_request_time = "00:00:00";
        if (!is_full_day) {
            let hour = $('#leave_start_hour').val();
            let minute = $('#leave_start_minute').val();
            start_request_time = hour + ":" + minute;
            end_request_time = $('#leave_stop_time').val();
        }
        let str = JSON.stringify({
            "emp_id": empId,
            "leave_type_id": leave_type_id,
            "leave_type_code": leave_type_code,
            "start_request_date": start_request_date,
            "end_request_date": end_request_date,
            "path_file": "",
            "status_request": "Pending",
            "description": description,
            "start_request_time": start_request_time,
            "end_request_time": end_request_time,
            "amount_leave_day": amount_leave_day,
            "is_two_step_approve": is_two_step_approve,
            "is_full_day": is_full_day,
            "amount_leave_hour": 0,
        });
        await $.ajax({
            type: "POST",
            url: leaveUrls.createRequest,
            contentType: 'application/x-www-form-urlencoded',
            data: { str, tempFileIds },
            success: function (response) {
                if (response === "Success") {
                    alert("ส่งวันลาเรียบร้อย");
                    $('#modal_confirm').modal('hide');
                    $('#modal_leave').modal('hide');
                } else {
                    alert(response);
                }
            }
        });
        location.reload();
        $('#SpinModal').modal('hide');
        $('#confirm_ok').attr('disabled', false);
    });

    function formatDate(date) {
        const daysThai = ['อา.', 'จ.', 'อ.', 'พ.', 'พฤ.', 'ศ.', 'ส.'];
        const dayOfWeek = daysThai[date.getDay()];
        const day = String(date.getDate()).padStart(2, '0');
        const month = String(date.getMonth() + 1).padStart(2, '0');
        const year = date.getFullYear();
        return `${dayOfWeek} ${day}/${month}/${year}`;
    }
}