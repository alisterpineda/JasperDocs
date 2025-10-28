import { Link } from '@tanstack/react-router';
import { Stack } from '@mantine/core';
import { IconFiles, IconUsers } from '@tabler/icons-react';
import classes from './Sidebar.module.css';

export function Sidebar() {
  return (
    <Stack gap="xs" p="md">
      <Link
        to="/documents"
        className={classes.link}
        activeProps={{
          className: `${classes.link} ${classes.active}`,
        }}
      >
        <IconFiles size={20} stroke={1.5} />
        <span>Documents</span>
      </Link>
      <Link
        to="/parties"
        className={classes.link}
        activeProps={{
          className: `${classes.link} ${classes.active}`,
        }}
      >
        <IconUsers size={20} stroke={1.5} />
        <span>Parties</span>
      </Link>
    </Stack>
  );
}
